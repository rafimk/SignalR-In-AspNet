using MediatR;
using SignalRWebApp.JwtAuthentications;

namespace SignalRWebApp.Commands;

public record SignInCommand : IRequest<JsonWebToken>
{
    public string MobileNumber { get; set; } = string.Empty;
}