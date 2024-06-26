﻿using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace SignalRWebApp.Hubs;


public class NotificationsHub : Hub<INotificationClient>
{
    private static readonly ConcurrentDictionary<string, string> _userConnectionMap = new ConcurrentDictionary<string, string>();
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NotificationsHub(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _httpContextAccessor?.HttpContext?.Items.First(x => x.Key.Equals("UserUniqueName")).Value;

        if (userId is not null)
        {
            _userConnectionMap.AddOrUpdate(userId.ToString()!, Context.ConnectionId, (key, value) => Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).ReceiveNotification($"Thank you for connecting {userId}");
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

    // Method to send a notification to a specific user
    public async Task SendNotificationToUser(string userId, string message)
    {
        if (userId is null)
        {
            return;
        }

        if (_userConnectionMap.TryGetValue(userId.ToString()!, out var connectionId))
        {
            await Clients.Client(connectionId).ReceiveNotification(message);
        }

        return;
    }
}