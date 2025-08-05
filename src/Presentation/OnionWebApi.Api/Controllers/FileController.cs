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
                ErrorMessage = "Dosya seçilmedi."
            });
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileUploadDto = new FileUploadDto
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                Content = memoryStream.ToArray()
            };

            var result = await _fileService.SaveFileAsync(fileUploadDto);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch
        {

            return StatusCode(500, new FileProcessResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Dosya yüklenirken bir hata oluştu."
            });
        }
    }

    [HttpGet("download/{*filePath}")]
    public async Task<IActionResult> DownloadFile(string filePath)
    {
        try
        {
            var file = await _fileService.GetFileAsync(filePath);
            if (file == null)
                return NotFound("Dosya bulunamadı.");

            return File(file.Content, file.ContentType, file.FileName);
        }
        catch
        {
            return StatusCode(500, "Dosya indirilirken bir hata oluştu.");
        }
    }

    [HttpDelete("{*filePath}")]
    public async Task<IActionResult> DeleteFile(string filePath)
    {
        try
        {
            var result = await _fileService.DeleteFileAsync(filePath);
            return !result ? NotFound("Dosya bulunamadı.") : NoContent();
        }
        catch
        {
            return StatusCode(500, "Dosya silinirken bir hata oluştu.");
        }
    }

    [HttpHead("{*filePath}")]
    public async Task<IActionResult> CheckFileExists(string filePath)
    {
        try
        {
            var exists = await _fileService.FileExistsAsync(filePath);
            return exists ? Ok() : NotFound();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPost("validate")]
    public async Task<ActionResult<FileValidationResultDto>> ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Dosya seçilmedi.");
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileUploadDto = new FileUploadDto
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                Content = memoryStream.ToArray()
            };

            var result = await _fileService.ValidateFileAsync(fileUploadDto);
            return Ok(result);
        }
        catch
        {
            return StatusCode(500, "Dosya doğrulanırken bir hata oluştu.");
        }
    }

    [HttpPost("move")]
    public async Task<IActionResult> MoveFile([FromBody] MoveFileRequestDto request)
    {
        try
        {
            var result = await _fileService.MoveFileAsync(request.SourcePath, request.DestinationPath);
            return result ? Ok() : BadRequest("Dosya taşınamadı.");
        }
        catch
        {
            return StatusCode(500, "Dosya taşınırken bir hata oluştu.");
        }
    }

    [HttpPost("copy")]
    public async Task<IActionResult> CopyFile([FromBody] CopyFileRequestDto request)
    {
        try
        {
            var result = await _fileService.CopyFileAsync(request.SourcePath, request.DestinationPath);
            return result ? Ok() : BadRequest("Dosya kopyalanamadı.");
        }
        catch
        {
            return StatusCode(500, "Dosya kopyalanırken bir hata oluştu.");
        }
    }
}
