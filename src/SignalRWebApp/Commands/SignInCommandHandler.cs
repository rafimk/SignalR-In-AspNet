using MediatR;
using SignalRWebApp.JwtAuthentications;
using SignalRWebApp.Services;

namespace SignalRWebApp.Commands;

public class SignInCommandHandler(IJwtTokenService jwtTokenService, IJsonWebTokenManager jsonWebTokenManager)
    : IRequestHandler<SignInCommand, JsonWebToken>
{
    public async Task<JsonWebToken> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var claims = new Dictionary<string, IEnumerable<string>>
        {
            ["permissions"] = new List<string> { "user" }
        };
        var jwt = jsonWebTokenManager.CreateToken(request.MobileNumber, "email", request.MobileNumber, claims: claims);
        var jwtToken = jwtTokenService.GenerateJwtToken(request.MobileNumber);
        await Task.CompletedTask;
        return jwt;
    }
}