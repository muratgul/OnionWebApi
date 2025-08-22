namespace OnionWebApi.Domain.Events.Brands;
public class BrandCreateDomainEvent : INotification
{
    public string Name { get; }

    public BrandCreateDomainEvent(string name)
    {
        Name = name;
    }
}
