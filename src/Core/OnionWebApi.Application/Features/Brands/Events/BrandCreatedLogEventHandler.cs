namespace OnionWebApi.Application.Features.Brands.Events;
public class BrandCreatedLogEventHandler : INotificationHandler<BrandCreateDomainEvent>
{
    public Task Handle(BrandCreateDomainEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("Brand created with Name: {Name}", notification.Name);

        return Task.CompletedTask;
    }
}
