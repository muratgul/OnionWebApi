namespace OnionWebApi.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommandRequest(int userId, string oldPassword, string newPassword) : ChangePasswordRequestDto(userId, oldPassword, newPassword),  IRequest;


public class ChangePasswordCommandHandler : BaseHandler, IRequestHandler<ChangePasswordCommandRequest>
{
    private readonly UserManager<AppUser> _userManager;
    public ChangePasswordCommandHandler(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _userManager = userManager;
    }

    public async Task Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new Exception("User not found");

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

        if(!result.Succeeded)
        {
            throw new Exception("Password change failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
