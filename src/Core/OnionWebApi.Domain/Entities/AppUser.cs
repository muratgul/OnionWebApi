namespace OnionWebApi.Domain.Entities;
public class AppUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
