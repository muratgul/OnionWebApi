namespace OnionWebApi.Application.Wrappers;
public class MassTransitMessage<T>
{
    public T Data { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
