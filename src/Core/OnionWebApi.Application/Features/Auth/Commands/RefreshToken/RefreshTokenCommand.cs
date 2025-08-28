namespace OnionWebApi.Application.Features.Auth.Commands.RefreshToken;
public class RefreshTokenCommandResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime RefreshTokenExpiryTime { get; set; }
}
public class RefreshTokenCommandRequest : IRequest<RefreshTokenCommandResponse>
{
}
internal class RefreshTokenCommandHandler : BaseHandler, IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
{
    private readonly AuthRules _authRules;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(IMapper mapper, AuthRules authRules, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, ITokenService tokenService, IUriService uriService, ICacheService cacheService, IConfiguration configuration) : base(mapper, unitOfWork, httpContextAccessor, uriService, cacheService)
    {
        _authRules = authRules;
        _userManager = userManager;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
    {
        var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var refreshTokenFromCookie = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];

        await _authRules.TokensShouldNotBeEmpty(accessToken, refreshTokenFromCookie);

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!);
        var email = principal!.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.FindByEmailAsync(email!);
        var roles = await _userManager.GetRolesAsync(user!);

        await _authRules.RefreshTokenShouldBeValid(user, refreshTokenFromCookie!);

        var newAccessToken = await _tokenService.CreateTokenAsync(user!, "", roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out var refreshTokenValidityInDays);

        user!.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

        await _userManager.UpdateAsync(user);

        return new()
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime.Value
        };
    }
}
