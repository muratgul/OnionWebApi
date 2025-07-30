namespace OnionWebApi.Application.Interfaces.Messaging;
public interface IEventPublisher
{
    Task PublishEventAsync(string eventName, object data);
}
