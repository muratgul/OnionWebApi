namespace OnionWebApi.Application.Services;
public class FileService(IFileStorageService fileStorageService, IConfiguration configuration) : IFileService
{
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly FileServiceOptions _options = configuration.GetSection("FileService").Get<FileServiceOptions>() ?? new FileServiceOptions();

    public async Task<FileProcessResultDto> SaveFileAsync(FileUploadDto fileUpload)
    {
        var validationResult = await ValidateFileAsync(fileUpload);
        if (!validationResult.IsValid)
        {
            return new FileProcessResultDto
            {
                IsSuccess = false,
                ErrorMessage = string.Join(", ", validationResult.Errors)
            };
        }

        var uniqueFileName = GenerateUniqueFileName(fileUpload.FileName);
        var subDirectory = DateTime.UtcNow.ToString("yyyy/MM/dd");
        var filePath = await _fileStorageService.SaveFileAsync(fileUpload.Content, uniqueFileName, subDirectory);

        return new FileProcessResultDto
        {
            IsSuccess = true,
            FilePath = filePath,
            GeneratedFileName = uniqueFileName,
            OriginalFileName = fileUpload.FileName,
        };
    }
    public async Task<FileDownloadDto?> GetFileAsync(string filePath)
    {
        if (!await _fileStorageService.FileExistsAsync(filePath))
        {
            return null;
        }

        var content = await _fileStorageService.GetFileContentAsync(filePath) ?? throw new FileNotFoundException(nameof(filePath));

        var fileName = Path.GetFileName(filePath);
        var contentType = await _fileStorageService.GetFileContentTypeAsync(filePath);

        return new FileDownloadDto
        {
            FileName = fileName,
            ContentType = contentType,
            Size = content.Length,
            Content = content
        };
    }
    public async Task<bool> DeleteFileAsync(string filePath)
    {
        var result = await _fileStorageService.DeleteFileAsync(filePath);
        
        if (result)
        {
            Log.Information("File deleted successfully: {FilePath}", filePath);
        }
        else
        {
            Log.Warning("Failed to delete file: {FilePath}", filePath);
        }

        return result;
    }
    public async Task<bool> FileExistsAsync(string filePath)
    {
        return await _fileStorageService.FileExistsAsync(filePath);
    }
    public async Task<long> GetFileSizeAsync(string filePath)
    {
        return await _fileStorageService.GetFileSizeAsync(filePath);
    }
    public async Task<FileValidationResultDto> ValidateFileAsync(FileUploadDto fileUpload)
    {
        var result = new FileValidationResultDto { IsValid = true };

        if (string.IsNullOrWhiteSpace(fileUpload.FileName))
        {
            result.Errors.Add("Dosya adı boş olamaz");
        }

        var extension = Path.GetExtension(fileUpload.FileName).ToLowerInvariant();
        if (_options.AllowedExtensions?.Any() == true && !_options.AllowedExtensions.Contains(extension))
        {
            result.Errors.Add($"Desteklenmeyen dosya uzantısı: {extension}");
        }

        if (fileUpload.Size == 0 || fileUpload.Content?.Length == 0)
        {
            result.Errors.Add("Dosya boş olamaz");
        }

        if (_options.AllowedContentTypes?.Any() == true && !_options.AllowedContentTypes.Contains(fileUpload.ContentType))
        {
            result.Errors.Add($"Desteklenmeyen dosya türü: {fileUpload.ContentType}");
        }

        result.IsValid = result.Errors.Count == 0;

        return await Task.FromResult(result);
    }
    public string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(extension);
        var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var guid = Guid.NewGuid().ToString("N")[..8];

        return $"{nameWithoutExtension}_{timeStamp}_{guid}{extension}";
    }    
    public async Task<bool> MoveFileAsync(string sourcePath, string destinationPath)
    {
        return await _fileStorageService.MoveFileAsync(sourcePath, destinationPath);
    }
    public async Task<bool> CopyFileAsync(string sourcePath, string destinationPath)
    {
        return await _fileStorageService.CopyFileAsync(sourcePath, destinationPath);
    }
    public async Task<string> GetFileContentTypeAsync(string filePath)
    {
        return await _fileStorageService.GetFileContentTypeAsync(filePath);
    }
    public async Task<byte[]?> GetFileBytesAsync(string filePath)
    {
        return await _fileStorageService.GetFileContentAsync(filePath);
    }  
}
