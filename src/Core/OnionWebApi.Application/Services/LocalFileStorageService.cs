namespace OnionWebApi.Application.Services;
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private static readonly ConcurrentDictionary<string, string> _contentTypes = new();
    static LocalFileStorageService()
    {
        // İçerik tiplerini statik olarak yükle
        var types = new (string Extension, string ContentType)[]
        {
            (".jpg", "image/jpeg"),
            (".jpeg", "image/jpeg"),
            (".png", "image/png"),
            (".gif", "image/gif"),
            (".pdf", "application/pdf"),
            (".doc", "application/msword"),
            (".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
            (".txt", "text/plain"),
            (".xml", "application/xml"),
            (".json", "application/json"),
            (".zip", "application/zip"),
            (".rar", "application/x-rar-compressed")
        };

        foreach (var (ext, type) in types)
        {
            _contentTypes[ext] = type;
        }
    }
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
        ValidateFileName(fileName);
        var directoryPath = GetValidDirectoryPath(subDirectory);
        var fullPath = Path.Combine(directoryPath, fileName);

        EnsureDirectoryExists(directoryPath);

        await File.WriteAllBytesAsync(fullPath, content);

        var relativePath = Path.GetRelativePath(_basePath, fullPath);

        return relativePath;       
    }
    public async Task<byte[]?> GetFileContentAsync(string filePath)
    {
        var fullPath = GetValidFilePath(filePath);
        return fullPath == null || !File.Exists(fullPath) ? null : await File.ReadAllBytesAsync(fullPath);
    }
    public async Task<bool> DeleteFileAsync(string filePath)
    {
        var fullPath = GetValidFilePath(filePath);
        if (fullPath == null || !File.Exists(fullPath))
        {
            return false;
        }

        await Task.Run(() => File.Delete(fullPath));
        Log.Information("File deleted: {FilePath}", filePath);
        return true;
    }
    public Task<bool> FileExistsAsync(string filePath)
    {
        var fullPath = GetValidFilePath(filePath);
        return Task.FromResult(fullPath != null && File.Exists(fullPath));
    }
    public Task<long> GetFileSizeAsync(string filePath)
    {
        var fullPath = GetValidFilePath(filePath);
        if (fullPath == null || !File.Exists(fullPath))
            return Task.FromResult(0L);

        var length = new FileInfo(fullPath).Length;
        return Task.FromResult(length);
    }
    public async Task<bool> MoveFileAsync(string sourcePath, string destinationPath)
    {
        var sourceFullPath = GetValidFilePath(sourcePath);
        var destFullPath = GetValidFilePath(destinationPath, ensureDirectory: true);

        if (!File.Exists(sourceFullPath))
        {
            return false;
        }

        if (sourceFullPath == null || !File.Exists(sourceFullPath))
        {
            return false;
        }

        EnsureDirectoryExists(Path.GetDirectoryName(destFullPath)!);

        await Task.Run(() => File.Move(sourceFullPath, destFullPath!));
        return true;
    }
    public async Task<bool> CopyFileAsync(string sourcePath, string destinationPath)
    {
        var sourceFullPath = GetValidFilePath(sourcePath);
        var destFullPath = GetValidFilePath(destinationPath, ensureDirectory: true);

        if (sourceFullPath == null || !File.Exists(sourceFullPath))
        {
            return false;
        }

        EnsureDirectoryExists(Path.GetDirectoryName(destFullPath)!);

        await Task.Run(() => File.Copy(sourceFullPath, destFullPath!, true));
        return true;
    }
    public Task<string> GetFileContentTypeAsync(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = _contentTypes.TryGetValue(extension, out var type) ? type : "application/octet-stream";
        return Task.FromResult(contentType);
    }
    public Task<IEnumerable<string>> GetFilesInDirectoryAsync(string directoryPath)
    {
        var fullPath = GetValidDirectoryPath(directoryPath);
        if (fullPath == null || !Directory.Exists(fullPath))
            return Task.FromResult<IEnumerable<string>>([]);

        var files = Directory.GetFiles(fullPath)
            .Select(f => Path.GetRelativePath(_basePath, f));

        return Task.FromResult<IEnumerable<string>>(files);
    }
    public async Task<bool> CreateDirectoryAsync(string directoryPath)
    {
        var fullPath = GetValidDirectoryPath(directoryPath);
        if (fullPath == null)
        {
            return false;
        }

        if (Directory.Exists(fullPath))
        {
            return true;
        }

        await Task.Run(() => Directory.CreateDirectory(fullPath));
        return true;
    }
    public async Task<bool> DeleteDirectoryAsync(string directoryPath, bool recursive = false)
    {
        var fullPath = GetValidDirectoryPath(directoryPath);
        if (fullPath == null || !Directory.Exists(fullPath))
        {
            return false;
        }

        await Task.Run(() => Directory.Delete(fullPath, recursive));
        return true;
    }
    private string? GetValidFilePath(string? relativePath, bool ensureDirectory = false)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        var fullPath = Path.GetFullPath(Path.Combine(_basePath, relativePath));
        if (!IsUnderBasePath(fullPath))
        {
            return null;
        }

        if (ensureDirectory)
        {
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir))
            {
                EnsureDirectoryExists(dir);
            }
        }

        return fullPath;
    }
    private string? GetValidDirectoryPath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return _basePath;
        }

        var fullPath = Path.GetFullPath(Path.Combine(_basePath, relativePath));
        return IsUnderBasePath(fullPath) ? fullPath : null;
    }
    private void ValidateFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        if (fileName.IndexOfAny(invalidChars) >= 0)
        {
            throw new ArgumentException("File name contains invalid characters.", nameof(fileName));
        }
    }
    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    private bool IsUnderBasePath(string path)
    {
        var baseDir = new DirectoryInfo(_basePath).FullName;
        var targetDir = new DirectoryInfo(path).FullName;
        return targetDir.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase);
    }
}
