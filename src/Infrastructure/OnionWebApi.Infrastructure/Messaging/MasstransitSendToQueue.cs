using MassTransit;
using OnionWebApi.Application.Wrappers;

namespace OnionWebApi.Infrastructure.Messaging;
public class MassTransitSendToQueue
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public MassTransitSendToQueue(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task SendToQueue<T>(T message, string queueName)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
        await endpoint.Send(new MassTransitMessage<T> { Content = message });
    }
}
