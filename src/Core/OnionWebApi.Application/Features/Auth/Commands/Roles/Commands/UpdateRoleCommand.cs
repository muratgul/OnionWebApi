namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Commands;
public class UpdateRoleCommandRequest : IRequest<Unit>
{
    public int Id { get; set; }
    public string RoleName { get; set; } = default!;
}

internal class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommandRequest, Unit>
{
    private readonly RoleManager<AppRole> _roleManager;
    public UpdateRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Unit> Handle(UpdateRoleCommandRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
        {
            throw new ArgumentNullException(nameof(request.RoleName));
        }

        var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (role is null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        role.Name = request.RoleName;

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.Select(x => x.Description).FirstOrDefault());
        }

        return Unit.Value;
    }
}
