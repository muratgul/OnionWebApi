namespace OnionWebApi.Domain.Models.Email;
public class EmailConfiguration
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public bool EnableSsl { get; set; }
    public bool UseDefaultCredentials { get; set; }
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromDisplayName { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public bool RetryEnabled { get; set; } = true;
    public int RetryCount { get; set; } = 3;
    public int RetryDelaySecond { get; set; } = 2;
    public int MaxConcurrentConnections { get; set; } = 10;
    public long MaxAttachmentSizeBytes { get; set; } = 25 * 1024 * 1024; // 25MB
}
