namespace OnionWebApi.Application.Interfaces.Tokens;
public interface ITokenService
{
    Task<JwtSecurityToken> CreateToken(AppUser user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
}
