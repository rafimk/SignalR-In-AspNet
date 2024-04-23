using MediatR;

namespace SignalRWebApp.Commands;

public record SignInCommand : IRequest<string>
{
    public string MobileNumber { get; set; } = string.Empty;
}