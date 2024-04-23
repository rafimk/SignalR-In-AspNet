namespace SignalRWebApp.Hubs;

public interface INotificationClient
{
    Task ReceiveNotification(string message);
}