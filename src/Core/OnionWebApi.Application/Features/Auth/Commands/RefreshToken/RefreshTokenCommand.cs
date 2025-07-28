namespace OnionWebApi.Application.Features.Auth.Commands.RefreshToken;
public class RefreshTokenCommandResponse
{
    public string AccessToken
    {
        get; set;
    }
    public string RefreshToken
    {
        get; set;
    }
}
public class RefreshTokenCommandRequest : IRequest<RefreshTokenCommandResponse>
{
    public string AccessToken
    {
        get; set;
    }
    public string RefreshToken
    {
        get; set;
    }
}
public class RefreshTokenCommandHandler : BaseHandler, IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
{
    private readonly AuthRules authRules;
    private readonly UserManager<User> userManager;
    private readonly ITokenService tokenService;
    public RefreshTokenCommandHandler(IMapper mapper, AuthRules authRules, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, ITokenService tokenService) : base(mapper, unitOfWork, httpContextAccessor)
    {
        this.authRules = authRules;
        this.userManager = userManager;
        this.tokenService = tokenService;
    }

    public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
    {
        ClaimsPrincipal? principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        string email = principal.FindFirstValue(ClaimTypes.Email);

        User? user = await userManager.FindByEmailAsync(email);
        IList<string> roles = await userManager.GetRolesAsync(user);

        await authRules.RefreshTokenShouldNotBeExpired(user.RefreshTokenExpiryTime);

        JwtSecurityToken newAccessToken = await tokenService.CreateToken(user, roles);
        string newRefreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await userManager.UpdateAsync(user);

        return new()
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
        };
    }
}
