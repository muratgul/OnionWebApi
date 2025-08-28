using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Application.Features.Auth.Commands.Revoke;
public class RevokeCommandRequest : IRequest<Unit>
{
    public string Email { get; set; } = default!;
}
internal class RevokeCommandHandler : BaseHandler, IRequestHandler<RevokeCommandRequest, Unit>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthRules _authRules;

    public RevokeCommandHandler(UserManager<AppUser> userManager, AuthRules authRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, cacheService)
    {
        _userManager = userManager;
        _authRules = authRules;
    }

    public async Task<Unit> Handle(RevokeCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        await _authRules.EmailAddressShouldBeValid(user);

        if(user is null)
        {
            throw new NotFoundException("User not found");
        }

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);        

        return Unit.Value;
    }
}
