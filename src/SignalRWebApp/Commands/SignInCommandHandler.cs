using MediatR;
using SignalRWebApp.Services;

namespace SignalRWebApp.Commands;

public class SignInCommandHandler(IJwtTokenService jwtTokenService)
    : IRequestHandler<SignInCommand, string>
{
    public async Task<string> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var jwtToken = jwtTokenService.GenerateJwtToken(request.MobileNumber);
        await Task.CompletedTask;
        return jwtToken;
    }
}