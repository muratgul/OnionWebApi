namespace OnionWebApi.Infrastructure.Tokens;
public class TokenSettings
{
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int TokenValidityInMunitues { get; set; }
}
