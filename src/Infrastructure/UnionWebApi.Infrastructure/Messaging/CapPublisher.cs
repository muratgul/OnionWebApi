using DotNetCore.CAP;

namespace UnionWebApi.Infrastructure.Messaging;
public class CapPublisher : IEventPublisher
{
    private readonly ICapPublisher _capPublisher;
    public CapPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }
    public void PublishEvent(string eventName, object data)
    {
        _capPublisher.Publish(eventName, data);
    }
}

