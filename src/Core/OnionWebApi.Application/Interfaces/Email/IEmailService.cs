namespace OnionWebApi.Application.Interfaces.Email;
public interface IEmailService
{
    Task<EmailResult> SendEmailAsync(EmailMessage message);
    Task<EmailResult> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task<EmailResult> SendEmailWithTemplateAsync<T>(string to, string templateName, T model);
    Task<EmailResult> SendEmailWithAttachmentsAsync(EmailMessage message, List<EmailAttachment> attachments);
    Task<EmailResult> SendScheduledEmailAsync(string to, string subject, string body, DateTime scheduledDate, bool isHtml = true);
    Task<BulkEmailResult> SendBulkEmailAsync(List<EmailMessage> messages, int maxConcurrency = 5);
    Task<EmailResult> SendEmailAsync(SendEmailRequestDto messageDto);
    Task<BulkEmailResult> SendBulkEmailAsync(BulkEmailRequestDto messagesDto, int maxConcurrency = 5);
}


