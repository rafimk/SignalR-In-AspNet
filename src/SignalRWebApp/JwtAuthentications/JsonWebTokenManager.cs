using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalRWebApp.Time;

namespace SignalRWebApp.JwtAuthentications;

internal sealed class JsonWebTokenManager : IJsonWebTokenManager
{
    private static readonly Dictionary<string, IEnumerable<string>> EmptyClaims = new();
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    private readonly string? _issuer;
    private readonly TimeSpan _expiry;
    private readonly IClock _clock;
    private readonly SigningCredentials _signingCredentials;
    private readonly string? _audience;

    public JsonWebTokenManager(IOptions<AuthOptions> options, SecurityKeyDetails securityKeyDetails, IClock clock)
    {
        if (options.Value?.Jwt is null)
        {
            throw new InvalidOperationException("Missing JWT options.");
        }

        _audience = options.Value.Jwt.Audience;
        _issuer = options.Value.Jwt.Issuer;
        _expiry = options.Value.Jwt.Expiry ?? TimeSpan.FromHours(1);
        _clock = clock;
        _signingCredentials = new SigningCredentials(securityKeyDetails.Key, securityKeyDetails.Algorithm);
    }

    public JsonWebToken CreateToken(string userId, string? email = null, string? role = null,
        IDictionary<string, IEnumerable<string>>? claims = null)
    {
        var now = _clock.Current();
        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.UniqueName, userId)
        };
        if (!string.IsNullOrWhiteSpace(email))
        {
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            jwtClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (!string.IsNullOrWhiteSpace(_audience))
        {
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, _audience));
        }

        if (claims?.Any() is true)
        {
            var customClaims = new List<Claim>();
            foreach (var (claim, values) in claims)
            {
                customClaims.AddRange(values.Select(value => new Claim(claim, value)));
            }

            jwtClaims.AddRange(customClaims);
        }

        var expires = now.Add(_expiry);

        var jwt = new JwtSecurityToken(
            _issuer,
            claims: jwtClaims,
            notBefore: now,
            expires: expires,
            signingCredentials: _signingCredentials
        );

        var token = _jwtSecurityTokenHandler.WriteToken(jwt);

        return new JsonWebToken
        {
            AccessToken = token,
            Expiry = new DateTimeOffset(expires).ToUnixTimeMilliseconds(),
            UserId = userId,
            Email = email ?? string.Empty,
            Role = role ?? string.Empty,
            Claims = claims ?? EmptyClaims
        };
    }
    
    public string? ParseUniqueNameFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        // Set up token validation parameters
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _signingCredentials.Key // Use the provided security key
        };

        try
        {
            // Read and validate the JWT token
            var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

            // Retrieve the UniqueName claim from the claims principal
            var uniqueNameClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.UniqueName);

            // Return the value of the UniqueName claim as a string
            return uniqueNameClaim?.Value;
        }
        catch (Exception ex)
        {
            // Token validation failed
            Console.WriteLine("Token validation failed: " + ex.Message);
            return null;
        }
    }
}