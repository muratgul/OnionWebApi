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
        var subDirectory = DateTime.UtcNow.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
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
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return null;
        }

        if (!await _fileStorageService.FileExistsAsync(filePath))
        {
            return null;
        }

        var content = await _fileStorageService.GetFileContentAsync(filePath) ?? throw new FileNotFoundException(nameof(filePath));

        if (content is null)
        {
            return null;
        }

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
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

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
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        return await _fileStorageService.FileExistsAsync(filePath);
    }
    public async Task<long> GetFileSizeAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return 0;
        }

        return await _fileStorageService.GetFileSizeAsync(filePath);
    }
    public async Task<FileValidationResultDto> ValidateFileAsync(FileUploadDto fileUpload)
    {
        var result = new FileValidationResultDto { IsValid = true };

        if (fileUpload == null)
        {
            result.Errors.Add("Dosya bilgisi boş olamaz.");
            result.IsValid = false;
            return result;
        }

        if (string.IsNullOrWhiteSpace(fileUpload.FileName))
        {
            result.Errors.Add("Dosya adı boş olamaz.");
        }

        var extension = Path.GetExtension(fileUpload.FileName).ToLowerInvariant();
        if (!string.IsNullOrEmpty(extension) &&
             _options.AllowedExtensions?.Any() == true &&
             !_options.AllowedExtensions.Contains(extension))
        {
            result.Errors.Add($"Desteklenmeyen dosya uzantısı: {extension}");
        }

        if (fileUpload.Content == null || fileUpload.Content.Length == 0)
        {
            result.Errors.Add("Dosya içeriği boş olamaz.");
        }

        if (!string.IsNullOrEmpty(fileUpload.ContentType) &&
            _options.AllowedContentTypes?.Any() == true &&
            !_options.AllowedContentTypes.Contains(fileUpload.ContentType))
        {
            result.Errors.Add($"Desteklenmeyen dosya türü: {fileUpload.ContentType}");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
    public string GenerateUniqueFileName(string originalFileName)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            throw new ArgumentException("Original file name cannot be null or empty.", nameof(originalFileName));
        }

        var extension = Path.GetExtension(originalFileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(extension);
        var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var guid = Guid.NewGuid().ToString("N")[..8];

        return $"{nameWithoutExtension}_{timeStamp}_{guid}{extension}";
    }    
    public async Task<bool> MoveFileAsync(string sourcePath, string destinationPath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || string.IsNullOrWhiteSpace(destinationPath))
        {
            return false;
        }

        return await _fileStorageService.MoveFileAsync(sourcePath, destinationPath);
    }
    public async Task<bool> CopyFileAsync(string sourcePath, string destinationPath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || string.IsNullOrWhiteSpace(destinationPath))
        {
            return false;
        }

        return await _fileStorageService.CopyFileAsync(sourcePath, destinationPath);
    }
    public async Task<string> GetFileContentTypeAsync(string filePath)
    {
        return string.IsNullOrWhiteSpace(filePath) ? "application/octet-stream" : await _fileStorageService.GetFileContentTypeAsync(filePath);
    }
    public async Task<byte[]?> GetFileBytesAsync(string filePath)
    {
        return string.IsNullOrWhiteSpace(filePath) ? null : await _fileStorageService.GetFileContentAsync(filePath);
    }
}
