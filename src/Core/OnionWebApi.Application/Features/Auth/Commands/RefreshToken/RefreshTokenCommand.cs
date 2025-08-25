using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Application.Features.Auth.Commands.RefreshToken;
public class RefreshTokenCommandResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
public class RefreshTokenCommandRequest : IRequest<RefreshTokenCommandResponse>
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
internal class RefreshTokenCommandHandler : BaseHandler, IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
{
    private readonly AuthRules _authRules;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    public RefreshTokenCommandHandler(IMapper mapper, AuthRules authRules, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, ITokenService tokenService, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _authRules = authRules;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var email = principal!.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.FindByEmailAsync(email!);
        var roles = await _userManager.GetRolesAsync(user!);

        await _authRules.RefreshTokenShouldNotBeExpired(user!.RefreshTokenExpiryTime);

        var newAccessToken = await _tokenService.CreateTokenAsync(user, password: "", roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new()
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
        };
    }
}
