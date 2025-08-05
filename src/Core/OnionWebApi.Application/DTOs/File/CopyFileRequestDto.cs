namespace OnionWebApi.Application.DTOs.File;
public class CopyFileRequestDto
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
}
