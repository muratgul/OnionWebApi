namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Commands;
public class DeleteUserRoleCommandRequest : IRequest
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}

internal class DeleteUserRoleCommandHandler : IRequestHandler<DeleteUserRoleCommandRequest>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public DeleteUserRoleCommandHandler(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Handle(DeleteUserRoleCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        
        if (user is null)
            throw new Exception("Kullanıcı bulunamadı.");

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
        
        if (role is null)
            throw new Exception("Rol bulunamadı.");

        var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);


        if (!result.Succeeded)
            throw new Exception($"Kullanı rolü silinemedi: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }
}
