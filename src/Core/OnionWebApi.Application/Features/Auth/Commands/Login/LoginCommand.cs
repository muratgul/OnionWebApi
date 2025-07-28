namespace OnionWebApi.Application.Features.Auth.Commands.Login;
public class LoginCommandResponse
{
    public string Token { get; set; } 
    public string RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
}

public class LoginCommandRequest : IRequest<LoginCommandResponse>
{
    [DefaultValue("cemkeskin12@gmail.com")]
    public string Email { get; set; }
    
    [DefaultValue("123456")] 
    public string Password { get; set; }
}

public class LoginCommandHandler : BaseHandler, IRequestHandler<LoginCommandRequest, LoginCommandResponse>
{
    private readonly UserManager<User> userManager;
    private readonly IConfiguration configuration;
    private readonly ITokenService tokenService;
    private readonly AuthRules authRules;

    public LoginCommandHandler(UserManager<User> userManager, IConfiguration configuration, ITokenService tokenService, AuthRules authRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
    {
        this.userManager = userManager;
        this.configuration = configuration;
        this.tokenService = tokenService;
        this.authRules = authRules;
    }
    public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
    {
        User user = await userManager.FindByEmailAsync(request.Email);
        bool checkPassword = await userManager.CheckPasswordAsync(user, request.Password);

        await authRules.EmailOrPasswordShouldNotBeInvalid(user, checkPassword);

        IList<string> roles = await userManager.GetRolesAsync(user);

        JwtSecurityToken token = await tokenService.CreateToken(user, roles);
        string refreshToken = tokenService.GenerateRefreshToken();

        _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

        await userManager.UpdateAsync(user);
        await userManager.UpdateSecurityStampAsync(user);

        string _token = new JwtSecurityTokenHandler().WriteToken(token);

        await userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", _token);

        return new()
        {
            Token = _token,
            RefreshToken = refreshToken,
            Expiration = token.ValidTo
        };

    }
}