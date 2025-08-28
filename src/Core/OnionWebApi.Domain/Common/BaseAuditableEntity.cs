namespace OnionWebApi.Domain.Common;
public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedDate { get; set; }
    public int CreatedUserId { get; set; }
    public DateTime? UpdatedDate { get; set; } = null;
    public int? UpdatedUserId { get; set; }


    // Navigation properties
    public virtual AppUser? CreatedUser { get; set; }
    public virtual AppUser? UpdatedUser { get; set; }
    

    // Computed properties
    public string CreatedUserName => CreatedUser?.FullName ?? "";
    public string? UpdatedUserName => UpdatedUser?.FullName;    
}
