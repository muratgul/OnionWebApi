namespace OnionWebApi.Application.Interfaces.Email;
public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailMessage message);
    Task<bool> SendEmailAsync(SendEmailRequestDto messageDto);
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task<Dictionary<EmailMessage, bool>> SendBulkEmailAsync(List<EmailMessage> messages);
    Task<Dictionary<EmailMessage, bool>> SendBulkEmailAsync(BulkEmailRequestDto messagesDto);
    Task<bool> SendEmailWithTemplateAsync<T>(string to, string templateName, T model);
    Task<bool> SendEmailWithAttachmentsAsync(EmailMessage message, List<EmailAttachment> attachments);
    Task<bool> SendEmailWithHeadersAsync(string to, string subject, string body, Dictionary<string, string> customHeaders, bool isHtml = true);
    Task<bool> SendScheduledEmailAsync(string to, string subject, string body, DateTime scheduledDate, bool isHtml = true);
}


