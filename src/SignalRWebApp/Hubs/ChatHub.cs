using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;

namespace SignalRWebApp.Hubs;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, string> _userConnections = new Dictionary<string, string>();
    
    public override async Task OnConnectedAsync()
    {
        var jwtToken = Context.GetHttpContext()?.Request.Query["jwtToken"];
        if (!string.IsNullOrEmpty(jwtToken))
        {
            // Validate and decode the JWT token to retrieve user information
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

            // Extract user's identifier from the JWT token
            var userIdentifier = jsonToken?.Payload["sub"]?.ToString();

            if (!string.IsNullOrEmpty(userIdentifier))
            {
                // Add user connection mapping logic here
            }
        }
        var userIdentifier2 = Context.User?.Identity?.Name; // Get user identifier from access token
        if (!string.IsNullOrEmpty(userIdentifier2))
        {
            _userConnections.TryAdd(userIdentifier2, Context.ConnectionId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _userConnections.Remove(Context.UserIdentifier, out _);
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task Register(string userName)
    {
        _userConnections.TryAdd(userName, Context.ConnectionId);
            
        await Clients.Caller.SendAsync("RegistrationComplete", "User registered successfully");
    }
    
    public async Task SendMessage(string user, string message)
    {
        _userConnections.TryAdd(user, Context.ConnectionId);
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
    
    public async Task SendNotification(string userId, string message)
    {
        // Get the connection ID of the user from the dictionary
        if (_userConnections.TryGetValue(userId, out string connectionId))
        {
            // Send the notification to the specific user
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
        else
        {
            // Handle scenario where user is not connected or connection information is not available
        }
    }
    
    
}
