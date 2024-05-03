using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalRWebApp.Hubs;

namespace SignalRWebApp.Commands;

public class ProcessCommandHandler : IRequestHandler<ProcessCommand, Unit>
{
    private readonly ILogger<ProcessCommandHandler> _logger;
    // private readonly IHubContext<NotificationsHub, INotificationClient> _context;
    private readonly IHubContext<ConnectionHub, Hub> _context;
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
        var userId = _httpContextAccessor?.HttpContext?.Items.First(x => x.Key.Equals("UserUniqueName")).Value;

        await _context.Clients.All.SendNotificationToUser(userId?.ToString()!, request.Message);

        return Unit.Value;
    }

}