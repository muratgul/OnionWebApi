namespace OnionWebApi.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommandResponse(string email, string token);

public record ForgotPasswordCommandRequest(string email) : ForgotPasswordDto(email), IRequest<ForgotPasswordCommandResponse>;

public class ForgotPasswordCommandHandler : BaseHandler, IRequestHandler<ForgotPasswordCommandRequest, ForgotPasswordCommandResponse>
{
    private readonly UserManager<AppUser> _userManager;
    public ForgotPasswordCommandHandler(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _userManager = userManager;
    }

    public async Task<ForgotPasswordCommandResponse> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.email) ?? throw new Exception("User is not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return new ForgotPasswordCommandResponse(request.email, token);
    }
}
