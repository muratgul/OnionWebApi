using MassTransit;
using OnionWebApi.Application.Interfaces.Messaging;
using OnionWebApi.Application.Wrappers;

namespace OnionWebApi.Infrastructure.Messaging;
public class MassTransitSend : IMassTransitSend
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitSend(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _publishEndpoint = publishEndpoint;
    }

    public async Task SendToExchange(object message, CancellationToken cancellationToken = default)
    {
        await _publishEndpoint.Publish(message);
    }

    public async Task SendToQueue<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
        await endpoint.Send(new MassTransitMessage<T> { Data = message }, cancellationToken);
    }

    public async Task SendToQueue<T>(T message, string queueName, Action<SendContext<T>> configureContext, CancellationToken cancellationToken = default) where T : class
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
        await endpoint.Send(message, configureContext, cancellationToken);
    }
}
