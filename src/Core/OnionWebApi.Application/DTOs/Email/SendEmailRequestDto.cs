namespace OnionWebApi.Application.DTOs.Email;
public class SendEmailRequestDto
{
    public string To { get; set; } = default!;
    public List<string> ToList { get; set; } = [];
    public List<string> CcList { get; set; } = [];
    public List<string> BccList { get; set; } = [];
    public string Subject { get; set; } = default!;
    public string Body { get; set; } = default!;
    public bool IsHtml { get; set; } = true;
    public string From { get; set; } = default!;
    public string? FromDisplayName { get; set; }
    public int Priority { get; set; } = 0;
    public DateTime? ScheduledDate { get; set; }
    public Dictionary<string, string> Headers { get; set; } = [];
}
