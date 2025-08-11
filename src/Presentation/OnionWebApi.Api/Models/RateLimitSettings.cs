namespace OnionWebApi.Api.Models;

public class RateLimitSettings
{
    public int PermitLimit { get; set; }
    public int WindowSeconds { get; set; }
    public int QueueLimit { get; set; }
    public bool Enabled { get; set; }
}
