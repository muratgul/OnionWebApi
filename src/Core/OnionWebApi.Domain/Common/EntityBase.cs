namespace OnionWebApi.Domain.Common;

public class EntityBase : IEntityBase
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
}
