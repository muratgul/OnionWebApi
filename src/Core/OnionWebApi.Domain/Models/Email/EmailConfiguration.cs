namespace OnionWebApi.Domain.Models.Email;
public class EmailConfiguration
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public bool EnableSsl { get; set; }
    public bool UseDefaultCredentials { get; set; }
    public string FromEmail { get; set; }
    public string FromDisplayName { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public bool EnableLogging { get; set; } = true;
    public int RetryCount { get; set; }
    public int RetryDelaySecond { get; set; }
}
