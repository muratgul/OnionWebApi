namespace OnionWebApi.Infrastructure.SignalR;
public class NotificationService : INotificationService
{
    private readonly IHubContext<GlobalHub> _hubContext;

    public NotificationService(IHubContext<GlobalHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task SendToAllAsync(string method, object payload)
    {
        return _hubContext.Clients.All.SendAsync(method, payload);
    }
    public Task SendToGroupAsync(string groupName, string method, object payload)
    {
        return _hubContext.Clients.Group(groupName).SendAsync(method, payload);
    }
    public Task SendToUserAsync(string userId, string method, object payload)
    {
        return _hubContext.Clients.User(userId).SendAsync(method, payload);
    }
}
