namespace OnionWebApi.Infrastructure.Messaging;
public class CapPublisher(ICapPublisher capPublisher) : IEventPublisher
{
    private readonly ICapPublisher _capPublisher = capPublisher;

    public void PublishEvent(string eventName, object data)
    {
        _capPublisher.Publish(eventName, data);
    }
}

