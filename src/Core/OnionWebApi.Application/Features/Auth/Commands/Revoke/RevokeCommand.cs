namespace OnionWebApi.Application.Features.Auth.Commands.Revoke;
public class RevokeCommandRequest : IRequest<Unit>
{
    public string Email { get; set; }
}
public class RevokeCommandHandler : BaseHandler, IRequestHandler<RevokeCommandRequest, Unit>
{
    private readonly UserManager<User> _userManager;
    private readonly AuthRules _authRules;

    public RevokeCommandHandler(UserManager<User> userManager, AuthRules authRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _userManager = userManager;
        _authRules = authRules;
    }

    public async Task<Unit> Handle(RevokeCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        await _authRules.EmailAddressShouldBeValid(user);

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return Unit.Value;
    }
}
