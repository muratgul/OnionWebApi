namespace OnionWebApi.Domain.Models.Email;
public class EmailTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string TextBody { get; set; } = string.Empty;
    public List<string> RequiredParameters { get; set; } = new();
}
