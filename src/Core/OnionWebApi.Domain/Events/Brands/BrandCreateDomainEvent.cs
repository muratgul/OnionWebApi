namespace OnionWebApi.Domain.Events.Brands;
public class BrandCreateDomainEvent : INotification
{
    public int Id { get; set; }
    public string Name { get; set; }

    public BrandCreateDomainEvent(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
