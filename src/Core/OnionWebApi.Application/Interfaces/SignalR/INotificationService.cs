namespace OnionWebApi.Application.Interfaces.SignalR;
public interface INotificationService
{
    Task SendToAllAsync(string method, object payload);
    Task SendToUserAsync(string userId, string method, object payload);
    Task SendToGroupAsync(string groupName, string method, object payload);
}
