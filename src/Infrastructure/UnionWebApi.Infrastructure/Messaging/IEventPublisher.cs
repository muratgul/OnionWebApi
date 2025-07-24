namespace UnionWebApi.Infrastructure.Messaging;
public interface IEventPublisher
{
    void PublishEvent(string eventName, object data);
}
