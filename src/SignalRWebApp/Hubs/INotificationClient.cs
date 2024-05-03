namespace SignalRWebApp.Hubs;

public interface INotificationClient
{
    Task ReceiveNotification(string message);
    Task SendNotificationToUser(string userId, string message);
}