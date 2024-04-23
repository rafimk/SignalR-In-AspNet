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
}