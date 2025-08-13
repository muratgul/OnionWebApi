namespace OnionWebApi.Api.Controllers;
public class EmailController : BaseController
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;

    public EmailController(IEmailService emailService, IEmailTemplateService templateService)
    {
        _emailService = emailService;
        _templateService = templateService;
    }

    [HttpPost("test")]
    public async Task<IActionResult> TestEmail()
    {
        try
        {
            var result = await _emailService.SendEmailAsync(
                "muratgul@gmail.com",
                "Test Email",
                "<h1>Bu bir test emailidir</h1><p>Email servisi çalışıyor!</p>");

            return Ok(new
            {
                success = result,
                message = result ? "Email sent" : "Email failed"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequestDto request)
    {
        var message = new EmailMessage
        {
            To = request.To,
            ToList = request.ToList ?? new List<string>(),
            CcList = request.CcList ?? new List<string>(),
            BccList = request.BccList ?? new List<string>(),
            Subject = request.Subject,
            Body = request.Body,
            IsHtml = request.IsHtml,
            From = request.From,
            FromDisplayName = request.FromDisplayName,
            Priority = request.Priority,
            ScheduledDate = request.ScheduledDate,
            Headers = request.Headers ?? new Dictionary<string, string>()
        };

        var result = await _emailService.SendEmailAsync(message);

        if (result)
        {
            return Ok(new
            {
                success = true,
                message = "Email send successfully"
            });
        }

        return BadRequest(new
        {
            success = false,
            message = "Failed to send email"
        });
    }

    [HttpPost("send-templated")]
    public async Task<IActionResult> SendTemplatedEmail([FromBody] SendTemplatedEmailRequestDto request)
    {
        var result = await _emailService.SendEmailWithTemplateAsync(
            request.To,
            request.TemplateName,
            request.TemplateData);

        if (result)
            return Ok(new
            {
                success = true,
                message = "Templated email sent successfully"
            });

        return BadRequest(new
        {
            success = false,
            message = "Failed to send templated email"
        });
    }

    [HttpPost("send-bulk")]
    public async Task<IActionResult> SendBulkEmail([FromBody] BulkEmailRequestDto request)
    {
        var messages = request.Emails.Select(e => new EmailMessage
        {
            To = e.To,
            ToList = e.ToList ?? new List<string>(),
            CcList = e.CcList ?? new List<string>(),
            BccList = e.BccList ?? new List<string>(),
            Subject = e.Subject,
            Body = e.Body,
            IsHtml = e.IsHtml,
            From = e.From,
            FromDisplayName = e.FromDisplayName,
            Priority = e.Priority,
            ScheduledDate = e.ScheduledDate,
            Headers = e.Headers ?? new Dictionary<string, string>()
        }).ToList();

        var result = await _emailService.SendBulkEmailAsync(messages);

        if (result.All(x => x.Value))
            return Ok(new
            {
                success = true,
                message = "Bulk emails sent successfully"
            });

        return BadRequest(new
        {
            success = false,
            message = "Some emails failed to send"
        });
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates()
    {
        var templates = await _templateService.GetAllTemplatesAsync();
        return Ok(templates);
    }

    [HttpPost("templates")]
    public async Task<IActionResult> SaveTemplate([FromBody] EmailTemplate template)
    {
        var result = await _templateService.SaveTemplateAsync(template);

        if (result)
            return Ok(new
            {
                success = true,
                message = "Template saved successfully"
            });

        return BadRequest(new
        {
            success = false,
            message = "Failed to save template"
        });
    }

    [HttpPost("test-advanced")]
    public async Task<IActionResult> TestAdvancedEmail()
    {
        try
        {
            var customHeaders = new Dictionary<string, string>
        {
            { "X-Custom-Header", "Test-Value" },
            { "X-Priority", "High" }
        };

            var message = new EmailMessage
            {
                To = "test@example.com",
                ToList = new List<string> { "test2@example.com" },
                CcList = new List<string> { "cc@example.com" },
                Subject = "Gelişmiş Test Email",
                Body = @"
                <div style='font-family: Arial, sans-serif;'>
                    <h1 style='color: #007bff;'>Gelişmiş Email Testi</h1>
                    <p>Bu email gelişmiş özelliklerle gönderilmiştir:</p>
                    <ul>
                        <li>Birden fazla alıcı</li>
                        <li>CC alıcıları</li>
                        <li>Özel header'lar</li>
                        <li>Yüksek öncelik</li>
                    </ul>
                    <p><strong>Gönderim Zamanı:</strong> {DateTime.Now:dd.MM.yyyy HH:mm}</p>
                </div>
            ",
                IsHtml = true,
                Priority = 1, // High priority
                Headers = customHeaders
            };

            var result = await _emailService.SendEmailAsync(message);

            return Ok(new
            {
                success = result,
                message = result ? "Gelişmiş email başarıyla gönderildi" : "Email gönderimi başarısız",
                details = new
                {
                    to = message.To,
                    toList = message.ToList,
                    ccList = message.CcList,
                    priority = message.Priority,
                    headers = message.Headers
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}

/*
 Örnekler:

// Basit email gönderimi
await _emailService.SendEmailAsync("user@example.com", "Test Subject", "<h1>Test Body</h1>");

// Template ile email gönderimi
await _emailService.SendEmailWithTemplateAsync("user@example.com", "welcome", new 
{ 
    UserName = "John Doe", 
    Email = "john@example.com" 
});

// Ek dosyalı email gönderimi
var attachments = new List<EmailAttachment>
{
    new EmailAttachment 
    { 
        FileName = "document.pdf", 
        Content = pdfBytes, 
        ContentType = "application/pdf" 
    }
};
await _emailService.SendEmailWithAttachmentsAsync(message, attachments);
 */