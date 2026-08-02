using OnionWebApi.Domain.Events.Brands;

namespace OnionWebApi.Domain.Entities;
public class Brand : BaseAuditableSoftDeletableEntity
{
    public Brand()
    {
    }
    public Brand(string name)
    {
        Name = name;
    }
    public string Name { get; set; } = string.Empty;

    //Event
    public void CreateBrand(string name)
    {
        Name = name;
        AddDomainEvent(new BrandCreateDomainEvent(name));
    }
}
