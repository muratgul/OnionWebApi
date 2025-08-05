using OnionWebApi.Domain.Models.Email;

namespace OnionWebApi.Application.Interfaces.Email;
public interface IEmailTemplateService
{
    Task<EmailTemplate> GetTemplateAsync(string templateName);
    string ProcessTemplate<T>(string template, T model);
    Task<bool> SaveTemplateAsync(EmailTemplate template);
    Task<List<EmailTemplate>> GetAllTemplatesAsync();
}
