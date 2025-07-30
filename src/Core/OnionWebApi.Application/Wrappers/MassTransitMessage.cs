namespace OnionWebApi.Application.Wrappers;
public class MassTransitMessage<T>
{
    public T Content
    {
        get; init;
    }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
