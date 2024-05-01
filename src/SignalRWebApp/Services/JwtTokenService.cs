using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalRWebApp.JwtAuthentications;

namespace SignalRWebApp.Services;

public class JwtTokenService(IOptions<JwtOptions> jwtOptions) : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateJwtToken(string mobileNumber)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            new[] {
                new Claim(ClaimTypes.Name, mobileNumber)
            },
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TokenExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GetUniqueNameFromToken(string? token)
    {
        try
        {
            return GetName(token);
        }
        catch (Exception e)
        {
            return "";
        }
    }
    
    private string GetName(string? token)
    {
        string secret = _jwtOptions.SecretKey;
        var key = Encoding.ASCII.GetBytes(secret);
        var handler = new JwtSecurityTokenHandler();
        var validations = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        var claims = handler.ValidateToken(token, validations, out var tokenSecure);
        return claims.Identity.Name;
    }
}