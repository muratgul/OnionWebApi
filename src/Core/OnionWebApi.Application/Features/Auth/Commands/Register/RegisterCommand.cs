using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Application.Features.Auth.Commands.Register;
public class RegisterCommandRequest : IRequest<Unit>
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
}
internal class RegisterCommandHandler : BaseHandler, IRequestHandler<RegisterCommandRequest, Unit>
{
    private readonly AuthRules _authRules;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public RegisterCommandHandler(AuthRules authRules, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, cacheService)
    {
        _authRules = authRules;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<Unit> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        await _authRules.UserShouldNotBeExist(await _userManager.FindByEmailAsync(request.Email));

        AppUser user = _mapper.Map<AppUser>(request);
        user.UserName = request.Email;
        user.SecurityStamp = Guid.NewGuid().ToString();

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync("user"))
                await _roleManager.CreateAsync(new AppRole
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