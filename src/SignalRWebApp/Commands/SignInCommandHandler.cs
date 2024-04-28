using MediatR;
using SignalRWebApp.JwtAuthentications;

namespace SignalRWebApp.Commands;

public class SignInCommandHandler(IJsonWebTokenManager jsonWebTokenManager)
    : IRequestHandler<SignInCommand, JsonWebToken>
{
    public async Task<JsonWebToken> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var claims = new Dictionary<string, IEnumerable<string>>
        {
            ["permissions"] = new List<string> { "user" }
        };
        var jwt = jsonWebTokenManager.CreateToken(request.MobileNumber, "email", request.MobileNumber, claims: claims);
        // var jwtToken = jwtTokenService.GenerateJwtToken(request.MobileNumber);
        await Task.CompletedTask;
        return jwt;
    }
}