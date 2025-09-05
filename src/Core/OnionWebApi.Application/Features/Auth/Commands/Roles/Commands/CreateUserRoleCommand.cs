namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Commands;
public record CreateUserRoleCommandRequest(int UserId, int RoleId) : IRequest<Unit>;

internal class CreateUserRoleCommandHandler : IRequestHandler<CreateUserRoleCommandRequest, Unit>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public CreateUserRoleCommandHandler(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Unit> Handle(CreateUserRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new Exception("Kullanıcı bulunamadı.");

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString()) ?? throw new Exception("Rol bulunamadı.");

        var result = await _userManager.AddToRoleAsync(user, role.Name!);

        return !result.Succeeded
            ? throw new Exception($"Kullanıcıya rol atanamadı: {string.Join(", ", result.Errors.Select(e => e.Description))}")
            : Unit.Value;
    }
}
