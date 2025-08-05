using OnionWebApi.Application.Interfaces.Email;
using OnionWebApi.Domain.Models.Email;

namespace OnionWebApi.Application.Services;
internal class EmailTemplateService : IEmailTemplateService
{
    private readonly Dictionary<string, EmailTemplate> _templates;

    public EmailTemplateService()
    {
        _templates = LoadDefaultTemplates();
    }
    public async Task<EmailTemplate> GetTemplateAsync(string templateName)
    {
        await Task.CompletedTask;
        return _templates.TryGetValue(templateName.ToLower(), out var template) ? template : null;
    }
    public string ProcessTemplate<T>(string template, T model)
    {
        if (string.IsNullOrEmpty(template) || model == null)
            return template;

        var result = template;
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(model)?.ToString() ?? string.Empty;
            result = result.Replace($"{{{{{property.Name}}}}}", value);
        }

        return result;
    }

    public async Task<bool> SaveTemplateAsync(EmailTemplate template)
    {
        try
        {
            await Task.CompletedTask; // Async for future database integration
            _templates[template.Name.ToLower()] = template;
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<List<EmailTemplate>> GetAllTemplatesAsync()
    {
        await Task.CompletedTask; // Async for future database integration
        return _templates.Values.ToList();
    }
    private Dictionary<string, EmailTemplate> LoadDefaultTemplates()
    {
        return new Dictionary<string, EmailTemplate>
        {
            ["welcome"] = new EmailTemplate
            {
                Name = "welcome",
                Subject = "Hoş Geldiniz {{UserName}}!",
                HtmlBody = @"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #333;'>Hoş Geldiniz {{UserName}}!</h1>
                        <p>Aramıza katıldığınız için teşekkür ederiz. Hesabınız başarıyla oluşturulmuştur.</p>
                        <p><strong>Email:</strong> {{Email}}</p>
                        <hr style='margin: 20px 0;'>
                        <p>Saygılarımızla,<br><strong>Ekibimiz</strong></p>
                    </div>
                ",
                TextBody = "Hoş Geldiniz {{UserName}}! Aramıza katıldığınız için teşekkür ederiz. Email: {{Email}}",
                RequiredParameters = new List<string> { "UserName", "Email" }
            },
            ["passwordreset"] = new EmailTemplate
            {
                Name = "passwordreset",
                Subject = "Şifre Sıfırlama Talebi",
                HtmlBody = @"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #d9534f;'>Şifre Sıfırlama</h1>
                        <p>Merhaba {{UserName}},</p>
                        <p>Şifre sıfırlama talebinde bulundunuz. Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
                        <p style='margin: 20px 0;'>
                            <a href='{{ResetLink}}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                                Şifreyi Sıfırla
                            </a>
                        </p>
                        <p>Bu bağlantı {{ExpirationHours}} saat içinde geçerliliğini yitirecektir.</p>
                        <p><small>Eğer bu talebi siz yapmadıysanız, lütfen bu e-postayı görmezden gelin.</small></p>
                        <hr style='margin: 20px 0;'>
                        <p>Saygılarımızla,<br><strong>Güvenlik Ekibi</strong></p>
                    </div>
                ",
                TextBody = "Merhaba {{UserName}}, Şifre sıfırlama bağlantısı: {{ResetLink}} Bu bağlantı {{ExpirationHours}} saat geçerlidir.",
                RequiredParameters = new List<string> { "UserName", "ResetLink", "ExpirationHours" }
            },
            ["emailverification"] = new EmailTemplate
            {
                Name = "emailverification",
                Subject = "Email Adresinizi Doğrulayın",
                HtmlBody = @"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #28a745;'>Email Doğrulama</h1>
                        <p>Merhaba {{UserName}},</p>
                        <p>Hesabınızı aktifleştirmek için email adresinizi doğrulamanız gerekmektedir.</p>
                        <p style='margin: 20px 0;'>
                            <a href='{{VerificationLink}}' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                                Email'i Doğrula
                            </a>
                        </p>
                        <p>Doğrulama kodu: <strong>{{VerificationCode}}</strong></p>
                        <p><small>Bu kod {{ExpirationMinutes}} dakika geçerlidir.</small></p>
                        <hr style='margin: 20px 0;'>
                        <p>Saygılarımızla,<br><strong>Ekibimiz</strong></p>
                    </div>
                ",
                TextBody = "Merhaba {{UserName}}, Email doğrulama bağlantısı: {{VerificationLink}} Doğrulama kodu: {{VerificationCode}}",
                RequiredParameters = new List<string> { "UserName", "VerificationLink", "VerificationCode", "ExpirationMinutes" }
            },
            ["notification"] = new EmailTemplate
            {
                Name = "notification",
                Subject = "{{NotificationTitle}}",
                HtmlBody = @"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #17a2b8;'>{{NotificationTitle}}</h1>
                        <p>Merhaba {{UserName}},</p>
                        <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #17a2b8; margin: 20px 0;'>
                            {{NotificationMessage}}
                        </div>
                        <p><small>Bu bildirim {{NotificationDate}} tarihinde gönderilmiştir.</small></p>
                        <hr style='margin: 20px 0;'>
                        <p>Saygılarımızla,<br><strong>Bildirim Sistemi</strong></p>
                    </div>
                ",
                TextBody = "{{NotificationTitle}} - Merhaba {{UserName}}, {{NotificationMessage}}",
                RequiredParameters = new List<string> { "UserName", "NotificationTitle", "NotificationMessage", "NotificationDate" }
            }
        };
    }

}
