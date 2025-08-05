using OnionWebApi.Application.DTOs.File;

namespace OnionWebApi.Application.Interfaces.File;
public interface IFileService
{
    Task<FileProcessResultDto> SaveFileAsync(FileUploadDto fileUpload);
    Task<FileDownloadDto?> GetFileAsync(string filePath);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    Task<long> GetFileSizeAsync(string filePath);
    Task<FileValidationResultDto> ValidateFileAsync(FileUploadDto fileUpload);
    string GenerateUniqueFileName(string originalFileName);
    Task<bool> MoveFileAsync(string sourcePath, string destinationPath);
    Task<bool> CopyFileAsync(string sourcePath, string destinationPath);
    Task<string> GetFileContentTypeAsync(string filePath);
    Task<byte[]?> GetFileBytesAsync(string filePath);
}
