using MediatR;
using SignalRWebApp.Auth;

namespace SignalRWebApp.Commands;

public class SignInCommandHandler(IAuthenticator _authenticator, ITokenStorage _tokenStorage)
    : IRequestHandler<SignInCommand, JwtDto>
{
    public async Task<JwtDto> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var isAddMember = false;
        var isAddUser = false;

        var jwt = _authenticator.CreateToken(request.UserId, "member", isAddMember, isAddUser);
        _tokenStorage.Set(jwt);
        await Task.CompletedTask;
        return jwt;
    }
}