namespace OnionWebApi.Application.Interfaces.File;
public interface IFileStorageService
{
    Task<string> SaveFileAsync(byte[] content, string fileName, string? subDirectory = null);
    Task<byte[]?> GetFileAsync(string filePath);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    Task<long> GetFileSizeAsync(string filePath);
    Task<bool> MoveFileAsync(string sourcePath, string destinationPath);
    Task<bool> CopyFileAsync(string sourcePath, string destinationPath);
    Task<string> GetFileContentTypeAsync(string filePath);
    string GetPhysicalPath(string relativePath);
    Task<IEnumerable<string>> GetFilesInDirectoryAsync(string directoryPath);
    Task<bool> CreateDirectoryAsync(string directoryPath);
    Task<bool> DeleteDirectoryAsync(string directoryPath, bool recursive = false);
}
