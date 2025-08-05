namespace OnionWebApi.Application.DTOs.File;
public class FileValidationResultDto
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
