namespace OnionWebApi.Application.Features.Auth.Commands.Register;
public class RegisterCommandRequest : IRequest<Unit>
{
    public string FullName  { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
public class RegisterCommandHandler : BaseHandler, IRequestHandler<RegisterCommandRequest, Unit>
{
    private readonly AuthRules _authRules;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public RegisterCommandHandler(AuthRules authRules, UserManager<AppUser> userManager, RoleManager<Role> roleManager, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _authRules = authRules;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<Unit> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        await _authRules.UserShouldNotBeExist(await _userManager.FindByEmailAsync(request.Email));

        AppUser user = _mapper.Map<AppUser, RegisterCommandRequest>(request);
        user.UserName = request.Email;
        user.SecurityStamp = Guid.NewGuid().ToString();

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync("user"))
                await _roleManager.CreateAsync(new Role
                {
                    Name = "user",
                    NormalizedName = "USER",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                });

            await _userManager.AddToRoleAsync(user, "user");
        }

        return Unit.Value;
    }
}