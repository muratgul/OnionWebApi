using OnionWebApi.Application.DTOs.File;
using OnionWebApi.Application.Helpers;

namespace OnionWebApi.Api.Controllers;
public class FileController : BaseController
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<FileProcessResultDto>> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new FileProcessResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Could not select file"
            });
        }

        var fileDto = await file.ToFileUploadDtoAsync();
        var result = await _fileService.SaveFileAsync(fileDto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);

    }

    [HttpGet("download/{*filePath}")]
    public async Task<IActionResult> DownloadFile(string filePath)
    {
        var file = await _fileService.GetFileAsync(filePath);
        return file is null ? NotFound(new FileProcessResultDto
        {
            IsSuccess = false,
            ErrorMessage = "File not found"
        }) : File(file.Content, file.ContentType, file.FileName);
    }

    [HttpDelete("{*filePath}")]
    public async Task<IActionResult> DeleteFile(string filePath)
    {
        var deleted = await _fileService.DeleteFileAsync(filePath);
        return deleted ? NoContent() : NotFound(new FileProcessResultDto
        {
            IsSuccess = false,
            ErrorMessage = "File not found"
        });
    }

    [HttpHead("{*filePath}")]
    public async Task<IActionResult> CheckFileExists(string filePath)
    {
        var exists = await _fileService.FileExistsAsync(filePath);
        return exists ? Ok() : NotFound(new FileProcessResultDto
        {
            IsSuccess = false,
            ErrorMessage = "File not found"
        });
    }

    [HttpPost("validate")]
    public async Task<ActionResult<FileValidationResultDto>> ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new FileProcessResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Could not validate file"
            });
        }

        var fileDto = await file.ToFileUploadDtoAsync();
        var result = await _fileService.ValidateFileAsync(fileDto);
        return result.IsValid ? Ok(result) : BadRequest(result);
    }

    [HttpPost("move")]
    public async Task<IActionResult> MoveFile([FromBody] MoveFileRequestDto request)
    {
        var result = await _fileService.MoveFileAsync(request.SourcePath, request.DestinationPath);
        return result ? Ok() : BadRequest(new FileProcessResultDto
        {
            IsSuccess = false,
            ErrorMessage = "Could not move file"
        });
    }

    [HttpPost("copy")]
    public async Task<IActionResult> CopyFile([FromBody] CopyFileRequestDto request)
    {
        var result = await _fileService.CopyFileAsync(request.SourcePath, request.DestinationPath);
        return result ? Ok() : BadRequest(new FileProcessResultDto
        {
            IsSuccess = false,
            ErrorMessage = "Could not copy file"
        });

    }
}
