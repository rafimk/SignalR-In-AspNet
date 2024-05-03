using MediatR;
using SignalRWebApp.Auth;

namespace SignalRWebApp.Commands;

public record SignInCommand : IRequest<JwtDto>
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
}