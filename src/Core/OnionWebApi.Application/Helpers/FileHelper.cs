namespace OnionWebApi.Application.Helpers;
public static class FileHelper
{
    public static async Task<FileUploadDto> ToFileUploadDtoAsync(this IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return new FileUploadDto
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Length,
            Content = memoryStream.ToArray()
        };
    }
}
