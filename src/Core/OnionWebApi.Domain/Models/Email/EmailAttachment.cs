namespace OnionWebApi.Domain.Models.Email;
public class EmailAttachment
{
    public string FileName { get; set; }
    public byte[] Content { get; set; }
    public string ContentType { get; set; }
    public Stream ContentStream { get; set; }
}
