namespace OnionWebApi.Infrastructure.Messaging;
public class MassTransitSend : IMassTransitSend
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly RabbitMQSettings _rabbitMQSettings;

    public MassTransitSend(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint, IOptions<RabbitMQSettings> rabbitMQSettings)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _publishEndpoint = publishEndpoint;
        _rabbitMQSettings = rabbitMQSettings.Value;
    }

    public async Task SendToExchange(object message, CancellationToken cancellationToken = default)
    {
        if (!_rabbitMQSettings.Enabled)
            return;

        try
        {
            await _publishEndpoint.Publish(message, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }

    public async Task SendToQueue<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        if (!_rabbitMQSettings.Enabled)
            return;

        try
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await endpoint.Send(new MassTransitMessage<T> { Data = message }, cancellationToken);
        }
        catch (Exception ex)

        {
            Log.Error(ex.Message);
        }
    }

    public async Task SendToQueue<T>(T message, string queueName, Action<SendContext<T>> configureContext, CancellationToken cancellationToken = default) where T : class
    {
        if (!_rabbitMQSettings.Enabled)
            return;

        try
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await endpoint.Send(message, configureContext, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }
}
