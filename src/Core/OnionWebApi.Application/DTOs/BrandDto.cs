namespace OnionWebApi.Application.DTOs;
public class BrandDto
{
    public string Name { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
