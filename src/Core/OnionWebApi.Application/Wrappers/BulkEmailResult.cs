namespace OnionWebApi.Application.Wrappers;
public class BulkEmailResult
{
    public int TotalEmails { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<(string Email, string Error)> Failures { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
}
