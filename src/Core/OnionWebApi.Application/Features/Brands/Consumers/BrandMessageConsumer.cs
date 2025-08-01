namespace OnionWebApi.Application.Features.Brands.Consumers;
public class BrandMessageConsumer : IConsumer<MassTransitMessage<string>>
{
    public Task Consume(ConsumeContext<MassTransitMessage<string>> context)
    {
        var message = context.Message.Data;
        Console.WriteLine($"Received message: {message}");
        return Task.CompletedTask;
    }
}
