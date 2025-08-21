using System.ComponentModel.DataAnnotations.Schema;

namespace OnionWebApi.Domain.Common;

public class BaseEntity : IEntityBase
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedUserId { get; set; }
    public DateTime? UpdatedDate { get; set; } = null;
    public int? UpdatedUserId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int? DeletedUserId { get; set; }

    // Navigation properties
    public virtual AppUser? CreatedUser {  get; set; }
    public virtual AppUser? UpdatedUser { get; set; }
    public virtual AppUser? DeletedUser { get; set; }
    
    // Computed properties
    public string CreatedUserName => CreatedUser?.FullName ?? "";
    public string? UpdatedUserName => UpdatedUser?.FullName;
    public string? DeletedUserName => DeletedUser?.FullName;

    private readonly List<INotification> _domainEvents = new List<INotification>();


    // Events
    [NotMapped]
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

}
