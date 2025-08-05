namespace OnionWebApi.Domain.Models.Email;
public class EmailTemplate
{
    public string Name { get; set; }
    public string Subject { get; set; }
    public string HtmlBody { get; set; }
    public string TextBody { get; set; }
    public List<string> RequiredParameters { get; set; } = new();
}
