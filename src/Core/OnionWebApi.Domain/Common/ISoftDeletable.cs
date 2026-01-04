namespace OnionWebApi.Domain.Common;
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    int? DeletedUserId { get; set; }
    DateTime? DeletedDate { get; set; }
}
