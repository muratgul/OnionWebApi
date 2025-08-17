namespace OnionWebApi.Application.Features.Auth.Commands.Login;
public class LoginCommandResponse
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime Expiration { get; set; }
}

public class LoginCommandRequest : IRequest<LoginCommandResponse>
{
    [Required]
    [EmailAddress]
    [DefaultValue("muratgul@gmail.com")]
    public string Email { get; set; } = default!;

    [Required]
    [DefaultValue("123456")] 
    public string Password { get; set; } = default!;
}

internal class LoginCommandHandler : BaseHandler, IRequestHandler<LoginCommandRequest, LoginCommandResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    private readonly AuthRules _authRules;

    public LoginCommandHandler(UserManager<AppUser> userManager, IConfiguration configuration, ITokenService tokenService, AuthRules authRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
        _authRules = authRules;
    }
    public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if(user is null)
        {
            await _authRules.EmailOrPasswordShouldNotBeInvalid(null, false);
        }

        var checkPassword = await _userManager.CheckPasswordAsync(user!, request.Password);

        await _authRules.EmailOrPasswordShouldNotBeInvalid(user, checkPassword);

        var roles = await _userManager.GetRolesAsync(user!);

        var token = await _tokenService.CreateTokenAsync(user!, password: "", roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out var refreshTokenValidityInDays);

        user!.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

        await _userManager.UpdateAsync(user);
        await _userManager.UpdateSecurityStampAsync(user);

        var _token = new JwtSecurityTokenHandler().WriteToken(token);

        await _userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", _token);

        return new()
        {
            Token = _token,
            RefreshToken = refreshToken,
            Expiration = token.ValidTo
        };

    }
}