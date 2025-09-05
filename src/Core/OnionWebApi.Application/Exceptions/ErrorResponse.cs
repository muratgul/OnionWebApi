namespace OnionWebApi.Application.Exceptions;
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public List<ErrorDetail> Errors { get; set; } = [];
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("O");
    public string? TraceId { get; set; }
}
