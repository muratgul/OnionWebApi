namespace OnionWebApi.Application.Features.Brands.Events;
public class BrandCreatedLogEventHandler : INotificationHandler<BrandCreateDomainEvent>
{
    public Task Handle(BrandCreateDomainEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("Brand created with ID: {Id} and Name: {Name}", notification.Id, notification.Name);

        return Task.CompletedTask;
    }
}
