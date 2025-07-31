using MassTransit;

namespace OnionWebApi.Application.Interfaces.Messaging;
public interface IMassTransitSend
{
    Task SendToExchange(object message, CancellationToken cancellationToken = default);
    Task SendToQueue<T>(T message, string queueName, CancellationToken cancellationToken) where T : class;
    Task SendToQueue<T>(T message, string queueName, Action<SendContext<T>> configureContext, CancellationToken cancellationToken = default) where T : class;
}
