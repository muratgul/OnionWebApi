namespace OnionWebApi.Domain.Models.Email;
public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public Stream ContentStream { get; set; } = Stream.Null;
}
