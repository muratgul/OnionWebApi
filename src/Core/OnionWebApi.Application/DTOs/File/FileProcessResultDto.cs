namespace OnionWebApi.Application.DTOs.File;
public class FileProcessResultDto
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FilePath { get; set; }
    public string? GeneratedFileName { get; set; }
    public string? OriginalFileName { get; set; }
    public long FileSize { get; set; }
}
