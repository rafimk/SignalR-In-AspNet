using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalRWebApp.Hubs;

namespace SignalRWebApp.Commands;

public class ProcessCommandHandler : IRequestHandler<ProcessCommand, Unit>
{
    private readonly ILogger<ProcessCommandHandler> _logger;
    private readonly IHubContext<NotificationsHub, INotificationClient> _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProcessCommandHandler(ILogger<ProcessCommandHandler> logger, 
        IHubContext<NotificationsHub, INotificationClient> context,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Unit> Handle(ProcessCommand request, CancellationToken cancellationToken)
    {
        string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
        await _context.Clients
            .User(request.UserId)
            .ReceiveNotification(request.Message);

        return Unit.Value;
    }
}