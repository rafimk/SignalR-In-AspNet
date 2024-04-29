using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalRWebApp.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        var uniqueName = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

        string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(token))
        {
            return Unit.Value;
        }

        _logger.LogInformation("Token received : ");

        if (string.IsNullOrEmpty(uniqueName))
        {
            return Unit.Value;
        }

        _logger.LogInformation($"unique_name : {uniqueName}");

        await _context.Clients
            .User(request.UserId)
            .ReceiveNotification(request.Message);
        
        return Unit.Value;
    }

    private string GetUniqueNameFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        // Read and parse the JWT token
        var jsonToken = handler.ReadJwtToken(token);

        // Retrieve the unique_name claim value
        var uniqueName = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

        return uniqueName;
    }
}