namespace OnionWebApi.Application.Services;
public class EmailService : IEmailService, IDisposable
{
    private readonly EmailConfiguration _emailConfig;
    private readonly IEmailTemplateService _templateService;
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed = false;
    public EmailService(
        IOptions<EmailConfiguration> emailConfig,
        IEmailTemplateService templateService)
    {
        _emailConfig = emailConfig.Value ?? throw new ArgumentNullException(nameof(emailConfig));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _semaphore = new SemaphoreSlim(_emailConfig.MaxConcurrentConnections);

        ValidateConfiguration();
    }

    public async Task<EmailResult> SendEmailAsync(EmailMessage message)
    {
        if (!_disposed)
        {
            try
            {
                var validationResult = ValidateEmailMessage(message);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                if (message.ScheduledDate.HasValue && message.ScheduledDate > DateTime.UtcNow)
                {
                    Log.Information("Email scheduled for {ScheduledDate}. Recipient: {To}", message.ScheduledDate, message.To);
                    return EmailResult.Success();
                }

                return await SendEmailInternalAsync(message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while sending email to {To}", message.To);
                return EmailResult.Failure(EmailSendResult.Failed, ex.Message, ex);
            }
        }

        throw new ObjectDisposedException(nameof(EmailService));
    }

    public async Task<EmailResult> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
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

    public async Task<EmailResult> SendScheduledEmailAsync(string to, string subject, string body, DateTime scheduledDate, bool isHtml = true)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml,
            ScheduledDate = scheduledDate
        };

        return await SendEmailAsync(message);
    }

    public async Task<BulkEmailResult> SendBulkEmailAsync(List<EmailMessage> messages, int maxConcurrency = 5)
    {
        if (!_disposed)
        {
            var startTime = DateTime.UtcNow;
            var result = new BulkEmailResult { TotalEmails = messages.Count };
            var failures = new ConcurrentBag<(string Email, string Error)>();

            var semaphore = new SemaphoreSlim(Math.Min(maxConcurrency, _emailConfig.MaxConcurrentConnections));

            try
            {
                var tasks = messages.Select(async message =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var emailResult = await SendEmailInternalAsync(message);
                        if (!emailResult.IsSuccess)
                        {
                            failures.Add((message.To, emailResult.ErrorMessage ?? "Unknown error"));
                            return false;
                        }
                        return true;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                var results = await Task.WhenAll(tasks);

                result.SuccessCount = results.Count(r => r);
                result.FailureCount = results.Count(r => !r);
                result.Failures = [.. failures];
                result.ProcessingTime = DateTime.UtcNow - startTime;

                Log.Information("Bulk email completed. Success: {Success}, Failed: {Failed}, Duration: {Duration}ms",
                    result.SuccessCount, result.FailureCount, result.ProcessingTime.TotalMilliseconds);

                return result;
            }
            finally
            {
                semaphore.Dispose();
            }
        }

        throw new ObjectDisposedException(nameof(EmailService));
    }

    public async Task<EmailResult> SendEmailWithTemplateAsync<T>(string to, string templateName, T model)
    {
        try
        {
            Log.Debug("Processing email template {TemplateName} for {To}", templateName, to);

            var template = await _templateService.GetTemplateAsync(templateName);
            if (template == null)
            {
                Log.Warning("Template {TemplateName} not found", templateName);
                return EmailResult.Failure(EmailSendResult.TemplateNotFound, $"Template '{templateName}' not found");
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
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing email template {TemplateName} for {To}", templateName, to);
            return EmailResult.Failure(EmailSendResult.Failed, ex.Message, ex);
        }
    }

    public async Task<EmailResult> SendEmailWithAttachmentsAsync(EmailMessage message, List<EmailAttachment> attachments)
    {
        try
        {
            // Validate attachments
            var totalSize = attachments.Sum(a => a.Content?.Length ?? 0);
            if (totalSize > _emailConfig.MaxAttachmentSizeBytes)
            {
                return EmailResult.Failure(EmailSendResult.AttachmentError,
                    $"Total attachment size ({totalSize} bytes) exceeds limit ({_emailConfig.MaxAttachmentSizeBytes} bytes)");
            }

            await _semaphore.WaitAsync();
            try
            {
                using var client = CreateSmtpClient();
                using var mailMessage = CreateMailMessage(message);

                // Add attachments
                foreach (var attachment in attachments)
                {
                    try
                    {
                        if (attachment.ContentStream != null)
                        {
                            mailMessage.Attachments.Add(new Attachment(attachment.ContentStream,
                                attachment.FileName, attachment.ContentType));
                        }
                        else if (attachment.Content != null)
                        {
                            var stream = new MemoryStream(attachment.Content);
                            mailMessage.Attachments.Add(new Attachment(stream,
                                attachment.FileName, attachment.ContentType));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error adding attachment {FileName}", attachment.FileName);
                        return EmailResult.Failure(EmailSendResult.AttachmentError,
                            $"Error adding attachment {attachment.FileName}: {ex.Message}");
                    }
                }

                await SendWithRetryAsync(client, mailMessage);

                Log.Information("Email with {AttachmentCount} attachments sent to {To}",
                    attachments.Count, message.To);

                return EmailResult.Success();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error sending email with attachments to {To}", message.To);
            return EmailResult.Failure(EmailSendResult.Failed, ex.Message, ex);
        }
    }

    public async Task<EmailResult> SendEmailAsync(SendEmailRequestDto messageDto)
    {
        var message = MapDtoToEmailMessage(messageDto);
        return await SendEmailAsync(message);
    }

    public async Task<BulkEmailResult> SendBulkEmailAsync(BulkEmailRequestDto messagesDto, int maxConcurrency = 5)
    {
        var messages = messagesDto.Emails.Select(MapDtoToEmailMessage).ToList();
        return await SendBulkEmailAsync(messages, maxConcurrency);
    }

    private async Task<EmailResult> SendEmailInternalAsync(EmailMessage message)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var client = CreateSmtpClient();
            using var mailMessage = CreateMailMessage(message);

            await SendWithRetryAsync(client, mailMessage);

            Log.Debug("Email sent successfully to {To}", message.To);
            return EmailResult.Success();
        }
        catch (SmtpException ex) when (ex.StatusCode == SmtpStatusCode.MailboxBusy ||
                                       ex.StatusCode == SmtpStatusCode.InsufficientStorage)
        {
            Log.Warning(ex, "SMTP server busy/full when sending to {To}", message.To);
            return EmailResult.Failure(EmailSendResult.NetworkError, ex.Message, ex);
        }
        catch (SmtpException ex) when (ex.StatusCode == SmtpStatusCode.MailboxUnavailable)
        {
            Log.Warning(ex, "Invalid recipient address: {To}", message.To);
            return EmailResult.Failure(EmailSendResult.ValidationFailed, "Invalid recipient address", ex);
        }
        catch (SmtpException ex)
        {
            Log.Error(ex, "SMTP error sending to {To}: {StatusCode}", message.To, ex.StatusCode);
            return EmailResult.Failure(EmailSendResult.NetworkError, ex.Message, ex);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error sending to {To}", message.To);
            return EmailResult.Failure(EmailSendResult.Failed, ex.Message, ex);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task SendWithRetryAsync(SmtpClient client, MailMessage mailMessage)
    {
        if (!_emailConfig.RetryEnabled)
        {
            await client.SendMailAsync(mailMessage);
            return;
        }

        var retryPolicy = Policy
            .Handle<SmtpException>(ex => ex.StatusCode != SmtpStatusCode.MailboxUnavailable) // Don't retry invalid addresses
            .Or<TimeoutException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(
                retryCount: _emailConfig.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(_emailConfig.RetryDelaySecond * retryAttempt),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Log.Warning("Email send attempt {RetryCount} failed: {Error}. Retrying in {Delay}s",
                        retryCount, exception.Message, timeSpan.TotalSeconds);
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            await client.SendMailAsync(mailMessage);
        });
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.SmtpPort)
        {
            EnableSsl = _emailConfig.EnableSsl,
            UseDefaultCredentials = _emailConfig.UseDefaultCredentials,
            Timeout = _emailConfig.TimeoutSeconds * 1000,
            DeliveryMethod = SmtpDeliveryMethod.Network
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
                !string.IsNullOrEmpty(message.From) ? message.From : _emailConfig.FromEmail,
                !string.IsNullOrEmpty(message.FromDisplayName) ? message.FromDisplayName : _emailConfig.FromDisplayName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };

        // Recipients
        AddRecipients(mailMessage.To, message.To, message.ToList);
        AddRecipients(mailMessage.CC, message.CcList);
        AddRecipients(mailMessage.Bcc, message.BccList);

        // Priority
        mailMessage.Priority = message.Priority switch
        {
            1 => MailPriority.High,
            -1 => MailPriority.Low,
            _ => MailPriority.Normal
        };

        // Headers
        foreach (var header in message.Headers ?? [])
        {
            mailMessage.Headers.Add(header.Key, header.Value);
        }

        return mailMessage;
    }

    private static void AddRecipients(MailAddressCollection collection, params object[] sources)
    {
        foreach (var source in sources)
        {
            switch (source)
            {
                case string email when !string.IsNullOrEmpty(email):
                    collection.Add(email);
                    break;
                case IEnumerable<string> emails:
                    foreach (var email in emails.Where(e => !string.IsNullOrEmpty(e)))
                    {
                        collection.Add(email);
                    }

                    break;
            }
        }
    }

    private EmailResult ValidateEmailMessage(EmailMessage message)
    {
        if (message is null)
        {
            return EmailResult.Failure(EmailSendResult.ValidationFailed, "Email message cannot be null");
        }

        if (string.IsNullOrWhiteSpace(message.To) && (message.ToList?.Count == 0 ))
        {
            return EmailResult.Failure(EmailSendResult.ValidationFailed, "At least one recipient is required");
        }

        if (string.IsNullOrWhiteSpace(message.Subject))
        {
            return EmailResult.Failure(EmailSendResult.ValidationFailed, "Subject is required");
        }

        return string.IsNullOrWhiteSpace(message.Body)
            ? EmailResult.Failure(EmailSendResult.ValidationFailed, "Body is required")
            : EmailResult.Success();
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_emailConfig.SmtpServer))
        {
            throw new InvalidOperationException("SMTP server is required");
        }

        if (_emailConfig.SmtpPort <= 0)
        {
            throw new InvalidOperationException("SMTP port must be greater than 0");
        }

        if (string.IsNullOrEmpty(_emailConfig.FromEmail))
        {
            throw new InvalidOperationException("From email is required");
        }
    }

    private static EmailMessage MapDtoToEmailMessage(SendEmailRequestDto dto)
    {
        return new EmailMessage
        {
            To = dto.To,
            ToList = dto.ToList ?? [],
            CcList = dto.CcList ?? [],
            BccList = dto.BccList ?? [],
            Subject = dto.Subject,
            Body = dto.Body,
            IsHtml = dto.IsHtml,
            From = dto.From,
            FromDisplayName = dto.FromDisplayName,
            Priority = dto.Priority,
            ScheduledDate = dto.ScheduledDate,
            Headers = dto.Headers ?? []
        };
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _semaphore?.Dispose();
            _disposed = true;
        }
    }
}
