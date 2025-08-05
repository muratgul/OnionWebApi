namespace OnionWebApi.Application.Services;
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    public LocalFileStorageService(IConfiguration configuration)
    {
        _basePath = configuration["FileStorage:BasePath"] ?? "uploads";

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }
    public async Task<string> SaveFileAsync(byte[] content, string fileName, string? subDirectory = null)
    {
        var directoryPath = string.IsNullOrEmpty(subDirectory)
                ? _basePath
                : Path.Combine(_basePath, subDirectory);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var fullPath = Path.Combine(directoryPath, fileName);
        await File.WriteAllBytesAsync(fullPath, content);

        var relativePath = Path.GetRelativePath(_basePath, fullPath);
        Log.Information("File saved to: {FilePath}", relativePath);

        return relativePath;
    }
    public async Task<byte[]?> GetFileAsync(string filePath)
    {
        var fullPath = GetPhysicalPath(filePath);
        return !File.Exists(fullPath) ? null : await File.ReadAllBytesAsync(fullPath);
    }
    public async Task<bool> DeleteFileAsync(string filePath)
    {
        var fullPath = GetPhysicalPath(filePath);
        if (!File.Exists(fullPath))
        {
            return false;
        }

        await Task.Run(() => File.Delete(fullPath));
        Log.Information("File deleted: {FilePath}", filePath);
        return true;
    }
    public async Task<bool> FileExistsAsync(string filePath)
    {
        var fullPath = GetPhysicalPath(filePath);
        return await Task.FromResult(File.Exists(fullPath));
    }
    public async Task<long> GetFileSizeAsync(string filePath)
    {
        var fullPath = GetPhysicalPath(filePath);
        if (!File.Exists(fullPath))
        {
            return 0;
        }

        var fileInfo = new FileInfo(fullPath);
        return await Task.FromResult(fileInfo.Length);
    }
    public async Task<bool> MoveFileAsync(string sourcePath, string destinationPath)
    {
        var sourceFullPath = GetPhysicalPath(sourcePath);
        var destinationFullPath = GetPhysicalPath(destinationPath);

        if (!File.Exists(sourceFullPath))
        {
            return false;
        }

        var destinationDir = Path.GetDirectoryName(destinationFullPath);
        if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        await Task.Run(() => File.Move(sourceFullPath, destinationFullPath));
        return true;
    }
    public async Task<bool> CopyFileAsync(string sourcePath, string destinationPath)
    {
        var sourceFullPath = GetPhysicalPath(sourcePath);
        var destinationFullPath = GetPhysicalPath(destinationPath);

        if (!File.Exists(sourceFullPath))
        {
            return false;
        }

        var destinationDir = Path.GetDirectoryName(destinationFullPath);
        if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        await Task.Run(() => File.Copy(sourceFullPath, destinationFullPath, true));
        return true;
    }
    public async Task<string> GetFileContentTypeAsync(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        var contentTypes = new Dictionary<string, string>
            {
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".gif", "image/gif" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".txt", "text/plain" },
                { ".xml", "application/xml" },
                { ".json", "application/json" },
                { ".zip", "application/zip" },
                { ".rar", "application/x-rar-compressed" }
            };

        return await Task.FromResult(contentTypes.GetValueOrDefault(extension, "application/octet-stream"));
    }
    public string GetPhysicalPath(string relativePath)
    {
        return Path.Combine(_basePath, relativePath);
    }
    public async Task<IEnumerable<string>> GetFilesInDirectoryAsync(string directoryPath)
    {
        var fullPath = GetPhysicalPath(directoryPath);
        if (!Directory.Exists(fullPath))
        {
            return Enumerable.Empty<string>();
        }

        var files = Directory.GetFiles(fullPath)
            .Select(f => Path.GetRelativePath(_basePath, f));

        return await Task.FromResult(files);
    }
    public async Task<bool> CreateDirectoryAsync(string directoryPath)
    {
        var fullPath = GetPhysicalPath(directoryPath);
        if (Directory.Exists(fullPath))
        {
            return true;
        }

        await Task.Run(() => Directory.CreateDirectory(fullPath));
        return true;
    }
    public async Task<bool> DeleteDirectoryAsync(string directoryPath, bool recursive = false)
    {
        var fullPath = GetPhysicalPath(directoryPath);
        if (!Directory.Exists(fullPath))
        {
            return false;
        }

        await Task.Run(() => Directory.Delete(fullPath, recursive));
        return true;
    }

}
