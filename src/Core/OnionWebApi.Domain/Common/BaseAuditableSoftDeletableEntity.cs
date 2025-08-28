namespace OnionWebApi.Domain.Common;
public abstract class BaseAuditableSoftDeletableEntity : BaseAuditableEntity, ISoftDeletable
{
    public bool IsDeleted { get; set; } = false;
    public int? DeletedUserId { get; set; }

    public virtual AppUser? DeletedUser { get; set; }
    public string? DeletedUserName => DeletedUser?.FullName;
}
