namespace OnionWebApi.Domain.Dtos;
public class EntityBaseDto
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedUserId { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int? UpdatedUserId { get; set; }
    public bool IsDeleted  { get; set; }
    public int? DeletedUserId { get; set; }
    public string CreatedUserName { get; set; } = "";
    public string? UpdatedUserName { get; set; }
}
