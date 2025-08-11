namespace OnionWebApi.Application.Services;
public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;
    private readonly IEmailTemplateService _templateService;

    public EmailService(IOptions<EmailConfiguration> emailConfig, IEmailTemplateService templateService)
    {
        _emailConfig = emailConfig.Value;
        _templateService = templateService;
    }
    public async Task<bool> SendEmailWithHeadersAsync(string to, string subject, string body, Dictionary<string, string> customHeaders, bool isHtml = true)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml,
            Headers = customHeaders ?? new Dictionary<string, string>()
        };

        return await SendEmailAsync(message);
    }
    public async Task<bool> SendScheduledEmailAsync(string to, string subject, string body, DateTime scheduledDate, bool isHtml = true)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml,
            ScheduledDate = scheduledDate
        };

        if (scheduledDate <= DateTime.UtcNow)
        {
            return await SendEmailAsync(message);
        }
        else
        {
            return true;
        }
    }
    public async Task<bool> SendEmailAsync(EmailMessage message)
    {
        using var client = CreateSmtpClient();
        using var mailMessage = CreateMailMessage(message);

        if (_emailConfig.RetryEnabled)
        {
            var retryPolicy = Policy
            .Handle<SmtpException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(retryCount: _emailConfig.RetryCount, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(_emailConfig.RetryDelaySecond),
            onRetry: (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"[Polly] {retryCount}. deneme başarısız: {exception.Message}");
            });

            await retryPolicy.ExecuteAsync(() => client.SendMailAsync(mailMessage));
        }
        else
        {
            await client.SendMailAsync(mailMessage);
        }

        return true;

    }
    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        };

        return await SendEmailAsync(message);
    }
    public async Task<Dictionary<EmailMessage, bool>> SendBulkEmailAsync(List<EmailMessage> messages)
    {
        var results = new Dictionary<EmailMessage, bool>();
        using var client = CreateSmtpClient();

        foreach (var message in messages)
        {
            try
            {
                using var mailMessage = CreateMailMessage(message);
                await client.SendMailAsync(mailMessage);
                results.Add(message, true);
            }
            catch
            {
                results.Add(message, false);
            }
        }

        return results;
    }
    public async Task<bool> SendEmailWithTemplateAsync<T>(string to, string templateName, T model)
    {
        try
        {
            var template = await _templateService.GetTemplateAsync(templateName);
            if (template is null)
            {
                return false;
            }

            var subject = _templateService.ProcessTemplate(template.Subject, model);
            var body = _templateService.ProcessTemplate(template.HtmlBody, model);

            var message = new EmailMessage
            {
                To = to,
                Subject = subject,
                Body = body,
                IsHtml = true
            };

            return await SendEmailAsync(message);
        }
        catch
        {
            return false;
        }
    }
    public async Task<bool> SendEmailWithAttachmentsAsync(EmailMessage message, List<EmailAttachment> attachments)
    {
        using var client = CreateSmtpClient();
        using var mailMessage = CreateMailMessage(message);

        foreach (var attachment in attachments)
        {
            if (attachment.ContentStream is not null)
            {
                mailMessage.Attachments.Add(new Attachment(attachment.ContentStream, attachment.FileName, attachment.ContentType));
            }
            else if (attachment.Content != null)
            {
                var stream = new MemoryStream(attachment.Content);
                mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
            }
        }

        await client.SendMailAsync(mailMessage);

        return true;
    }
    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.SmtpPort)
        {
            EnableSsl = _emailConfig.EnableSsl,
            UseDefaultCredentials = _emailConfig.UseDefaultCredentials,
            Timeout = _emailConfig.TimeoutSeconds * 1000
        };

        if (!_emailConfig.UseDefaultCredentials && !string.IsNullOrEmpty(_emailConfig.SmtpUsername))
        {
            client.Credentials = new NetworkCredential(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword);
        }

        return client;
    }
    private MailMessage CreateMailMessage(EmailMessage message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(
                string.IsNullOrEmpty(message.From) ? _emailConfig.FromEmail : message.From,
                string.IsNullOrEmpty(message.FromDisplayName) ? _emailConfig.FromDisplayName : message.FromDisplayName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };

        // To recipients
        if (!string.IsNullOrEmpty(message.To))
            mailMessage.To.Add(message.To);

        foreach (var to in message.ToList)
            mailMessage.To.Add(to);

        // CC recipients
        foreach (var cc in message.CcList)
            mailMessage.CC.Add(cc);

        // BCC recipients
        foreach (var bcc in message.BccList)
            mailMessage.Bcc.Add(bcc);

        // Priority
        mailMessage.Priority = message.Priority switch
        {
            1 => MailPriority.High,
            -1 => MailPriority.Low,
            _ => MailPriority.Normal
        };

        // Custom headers
        foreach (var header in message.Headers)
        {
            mailMessage.Headers.Add(header.Key, header.Value);
        }

        return mailMessage;
    }

}
