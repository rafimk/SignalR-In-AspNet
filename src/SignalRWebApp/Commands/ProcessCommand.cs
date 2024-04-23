using MediatR;

namespace SignalRWebApp.Commands;

public record ProcessCommand : IRequest<Unit>
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}