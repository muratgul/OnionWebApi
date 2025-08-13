namespace OnionWebApi.Application.SignalR.Messages;
public class NotificationMessage
{
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SendAt { get; set; } = DateTime.UtcNow;
}
