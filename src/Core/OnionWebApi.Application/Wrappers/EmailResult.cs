namespace OnionWebApi.Application.Wrappers;
public class EmailResult
{
    public EmailSendResult Result { get; set; }
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
    public bool IsSuccess => Result == EmailSendResult.Success;
    public static EmailResult Success() => new() { Result = EmailSendResult.Success };
    public static EmailResult Failure(EmailSendResult result, string? message = null, Exception? exception = null) =>
        new()
        {
            Result = result,
            ErrorMessage = message,
            Exception = exception
        };
}
