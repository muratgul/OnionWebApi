namespace OnionWebApi.Application.DTOs;
public class BrandDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? NameId { get; set; }
}
