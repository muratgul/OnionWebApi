namespace OnionWebApi.Application.Services;
public class EmailTemplateService : IEmailTemplateService
{
    private readonly ConcurrentDictionary<string, EmailTemplate> _templates;

    public EmailTemplateService()
    {
        _templates = new ConcurrentDictionary<string, EmailTemplate>(LoadDefaultTemplates());
    }
    public Task<EmailTemplate?> GetTemplateAsync(string templateName)
    {
        return string.IsNullOrWhiteSpace(templateName)
            ? Task.FromResult<EmailTemplate?>(null)
            : Task.FromResult<EmailTemplate?>(_templates.TryGetValue(templateName.ToLowerInvariant(), out var template) ? template : null);
    }
    public Task<List<EmailTemplate>> GetAllTemplatesAsync()
    {
        return Task.FromResult(_templates.Values.ToList());
    }
    public Task<bool> SaveTemplateAsync(EmailTemplate template)
    {
        if (template?.Name == null)
        {
            return Task.FromResult(false);
        }

        try
        {
            _templates[template.Name.ToLowerInvariant()] = template;
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
    public string ProcessTemplate<T>(string template, T model)
    {
        if (string.IsNullOrEmpty(template) || model == null)
        {
            return template;
        }

        var result = template;
        var properties = GetPropertiesCache<T>.Properties;

        foreach (var property in properties)
        {
            var value = property.Getter(model)?.ToString() ?? string.Empty;
            result = result.Replace($"{{{{{property.Name}}}}}", value, StringComparison.Ordinal);
        }

        return result;
    }    
    private Dictionary<string, EmailTemplate> LoadDefaultTemplates()
    {
        return new Dictionary<string, EmailTemplate>(StringComparer.OrdinalIgnoreCase)
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
                RequiredParameters = ["UserName", "Email"]
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
                RequiredParameters = [ "UserName", "ResetLink", "ExpirationHours" ]
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
                RequiredParameters = [ "UserName", "VerificationLink", "VerificationCode", "ExpirationMinutes" ]
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
                RequiredParameters = [ "UserName", "NotificationTitle", "NotificationMessage", "NotificationDate" ]
            }
        };
    }

}
internal static class GetPropertiesCache<T>
{
    public static readonly (string Name, Func<T, object?> Getter)[] Properties;

    static GetPropertiesCache()
    {
        var type = typeof(T);
        var instance = Expression.Parameter(typeof(T), "instance");
        var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var props = new (string Name, Func<T, object?> Getter)[propertyInfos.Length];

        for (int i = 0; i < propertyInfos.Length; i++)
        {
            var prop = propertyInfos[i];
            var cast = Expression.Convert(Expression.Property(instance, prop), typeof(object));
            props[i] = (prop.Name, Expression.Lambda<Func<T, object?>>(cast, instance).Compile());
        }

        Properties = props;
    }
}
