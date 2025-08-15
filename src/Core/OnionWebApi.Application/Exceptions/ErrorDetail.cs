namespace OnionWebApi.Application.Exceptions;
public class ErrorDetail
{
    public string? Field { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Code { get; set; }
}