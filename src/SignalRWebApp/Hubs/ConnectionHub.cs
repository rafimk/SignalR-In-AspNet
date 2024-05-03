using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace SignalRWebApp.Hubs;

public class ConnectionHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> _userConnectionMap = new ConcurrentDictionary<string, string>();

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ConnectionHub(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Send(string userId)
    {
        var message = $"Send message to you with user id {userId}";
        await Clients.Client(userId).SendAsync("ReceiveNotification", message);
    }

    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _httpContextAccessor?.HttpContext?.Items.First(x => x.Key.Equals("UserUniqueName")).Value;

        if (userId is not null)
        {
            _userConnectionMap.AddOrUpdate(userId.ToString()!, Context.ConnectionId, (key, value) => Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveNotification", $"Thank you for connecting {userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Get the unique identifier of the user (e.g., user ID) from the context
        var userId = _httpContextAccessor?.HttpContext?.Items.First(x => x.Key.Equals("UserUniqueName")).Value;

        if (userId is not null)
        {
            _userConnectionMap.TryRemove(userId.ToString()!, out _);
        }

        await base.OnDisconnectedAsync(exception);
    }
}