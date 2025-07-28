namespace OnionWebApi.Application.Features.Auth.Commands.RevokeAll;
public class RevokeAllCommandRequest : IRequest<Unit>
{
}
public class RevokeAllCommandHandler : BaseHandler, IRequestHandler<RevokeAllCommandRequest, Unit>
{
    private readonly UserManager<User> _userManager;
    public RevokeAllCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, UserManager<User> userManager) : base(mapper, unitOfWork, httpContextAccessor, uriService)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(RevokeAllCommandRequest request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }

        return Unit.Value;
    }
}