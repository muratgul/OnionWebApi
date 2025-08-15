namespace OnionWebApi.Application.DTOs.File;
public class FileUploadDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public byte[] Content { get; set; } = [];
}
