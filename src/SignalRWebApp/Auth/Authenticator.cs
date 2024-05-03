using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalRWebApp.Time;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignalRWebApp.Auth;

internal sealed class Authenticator : IAuthenticator
{
    private readonly IClock _clock;
    private readonly string _issuer;
    private readonly TimeSpan _expiry;
    private readonly string _audience;
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _jwtSecurityToken = new JwtSecurityTokenHandler();

    public Authenticator(IOptions<AuthOptions> options, IClock clock)
    {
        _clock = clock;
        _issuer = options.Value.Issuer;
        _audience = options.Value.Audience;
        _expiry = TimeSpan.FromHours(3);
        _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(options.Value.SigningKey)),
                SecurityAlgorithms.HmacSha256);
    }

    public JwtDto CreateToken(Guid userId, string role)
    {
        var now = _clock.Current();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new(ClaimTypes.Role, role)
        };

        var expires = now.Add(_expiry);
        var jwt = new JwtSecurityToken(_issuer, _audience, claims, now, expires, _signingCredentials);
        var token = _jwtSecurityToken.WriteToken(jwt);

        return new JwtDto
        {
            AccessToken = token
        };
    }

    public JwtDto CreateToken(Guid userId, string role, bool isAddMember, bool isAddUser)
    {
        var now = _clock.Current();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new(ClaimTypes.Name, userId.ToString()),
            new(ClaimTypes.Role, role),
            new("IsAddMember", isAddMember ? "true" : "false"),
            new("IsAddUser", isAddUser ? "true" : "false")
        };

        var expires = now.Add(_expiry);
        var jwt = new JwtSecurityToken(_issuer, _audience, claims, now, expires, _signingCredentials);
        var token = _jwtSecurityToken.WriteToken(jwt);

        return new JwtDto
        {
            AccessToken = token
        };
    }

    public JwtDto CreateToken(string uniqueName, string role, bool isAddMember, bool isAddUser)
    {
        var now = _clock.Current();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, uniqueName),
            new(JwtRegisteredClaimNames.UniqueName, uniqueName),
            new(ClaimTypes.Role, role),
            new("IsAddMember", isAddMember ? "true" : "false"),
            new("IsAddUser", isAddUser ? "true" : "false")
        };

        var expires = now.Add(_expiry);
        var jwt = new JwtSecurityToken(_issuer, _audience, claims, now, expires, _signingCredentials);
        var token = _jwtSecurityToken.WriteToken(jwt);

        return new JwtDto
        {
            AccessToken = token
        };
    }

    public string GetUniqueNameFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        // Read and validate the token
        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (securityToken == null)
        {
            throw new ArgumentException("Invalid token.");
        }

        // Extract the claims
        var claims = securityToken.Claims;

        // Find the UniqueName claim and return its value
        var uniqueNameClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);

        if (uniqueNameClaim == null)
        {
            throw new InvalidOperationException("UniqueName claim not found in token.");
        }

        return uniqueNameClaim.Value;
    }
}