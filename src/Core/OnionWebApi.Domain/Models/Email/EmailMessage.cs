namespace OnionWebApi.Domain.Models.Email;
public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public List<string> ToList { get; set; } = new List<string>();
    public List<string> CcList { get; set; } = new List<string>();
    public List<string> BccList { get; set; } = new List<string>();
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public string From { get; set; } = string.Empty;
    public string FromDisplayName { get; set; } = string.Empty;
    public int Priority { get; set; } = 0; //0: Normal, 1: High, -1: Low
    public DateTime? ScheduledDate { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();

}
