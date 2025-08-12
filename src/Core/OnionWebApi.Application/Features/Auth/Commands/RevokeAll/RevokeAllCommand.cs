namespace OnionWebApi.Application.Features.Auth.Commands.RevokeAll;
public class RevokeAllCommandRequest : IRequest
{
}
public class RevokeAllCommandHandler : BaseHandler, IRequestHandler<RevokeAllCommandRequest>
{
    private readonly UserManager<AppUser> _userManager;
    public RevokeAllCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, UserManager<AppUser> userManager, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _userManager = userManager;
    }

    public async Task Handle(RevokeAllCommandRequest request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }
    }
}