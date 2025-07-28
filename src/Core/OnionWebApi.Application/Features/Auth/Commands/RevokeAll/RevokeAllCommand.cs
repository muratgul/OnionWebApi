namespace OnionWebApi.Application.Features.Auth.Commands.RevokeAll;
public class RevokeAllCommandRequest : IRequest<Unit>
{
}
public class RevokeAllCommandHandler : BaseHandler, IRequestHandler<RevokeAllCommandRequest, Unit>
{
    private readonly UserManager<User> userManager;
    public RevokeAllCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager) : base(mapper, unitOfWork, httpContextAccessor)
    {
        this.userManager = userManager;
    }

    public async Task<Unit> Handle(RevokeAllCommandRequest request, CancellationToken cancellationToken)
    {
        List<User> users = await userManager.Users.ToListAsync(cancellationToken);

        foreach (User user in users)
        {
            user.RefreshToken = null;
            await userManager.UpdateAsync(user);
        }

        return Unit.Value;
    }
}