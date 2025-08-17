namespace OnionWebApi.Application.Interfaces.Tokens;
public interface ITokenService
{
    Task<JwtSecurityToken> CreateTokenAsync(AppUser user, string password, IList<string> roles, CancellationToken cancellationToken = default);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
}
