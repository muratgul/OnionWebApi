namespace OnionWebApi.Application.DTOs.Email;

public class SendTemplatedEmailRequestDto
{
    public string To { get; set; }
    public string TemplateName { get; set; }
    public Dictionary<string, object> TemplateData { get; set; } = [];
}
