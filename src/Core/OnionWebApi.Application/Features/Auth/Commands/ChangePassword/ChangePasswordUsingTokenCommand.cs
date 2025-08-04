namespace OnionWebApi.Application.Features.Auth.Commands.ChangePassword;
public class ChangePasswordUsingTokenCommandRequest : IRequest<Unit>
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}

public class ChangePasswordUsingTokenCommandRequestHandler : BaseHandler, IRequestHandler<ChangePasswordUsingTokenCommandRequest, Unit>
{
    private readonly UserManager<AppUser> _userManager;
    public ChangePasswordUsingTokenCommandRequestHandler(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(ChangePasswordUsingTokenCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new NotFoundException("User is not found");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            throw new Exception("Password change failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Unit.Value;
    }
}
