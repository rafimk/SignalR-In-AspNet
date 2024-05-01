using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalRWebApp.Hubs;
using SignalRWebApp.JwtAuthentications;

namespace SignalRWebApp.Commands;

public class ProcessCommandHandler : IRequestHandler<ProcessCommand, Unit>
{
    private readonly ILogger<ProcessCommandHandler> _logger;
    private readonly IHubContext<NotificationsHub, INotificationClient> _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private IJsonWebTokenManager _jsonWebTokenManager;

    public ProcessCommandHandler(ILogger<ProcessCommandHandler> logger, 
        IHubContext<NotificationsHub, INotificationClient> context,
        IHttpContextAccessor httpContextAccessor,
        IJsonWebTokenManager jsonWebTokenManager)
    {
        _logger = logger;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _jsonWebTokenManager = jsonWebTokenManager;
    }

    public async Task<Unit> Handle(ProcessCommand request, CancellationToken cancellationToken)
    {
        // var uniqueName = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

        string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

        var uniqueName = _jsonWebTokenManager.ParseUniqueNameFromToken(token!);
        
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

}