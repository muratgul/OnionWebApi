namespace OnionWebApi.Application.DTOs.Email;

public class SendTemplatedEmailRequestDto
{
    public string To { get; set; } = default!;
    public string TemplateName { get; set; } = default!;
    public Dictionary<string, object> TemplateData { get; set; } = [];
}
