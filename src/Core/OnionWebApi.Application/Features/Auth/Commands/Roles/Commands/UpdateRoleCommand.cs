using Microsoft.AspNetCore.Identity;

namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Commands;
public class UpdateRoleCommandRequest : IRequest<Unit>
{
    public int Id { get; set; }
    public string RoleName { get; set; }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommandRequest, Unit>
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
            throw new ArgumentException(nameof(role));
        }

        role.Name = request.RoleName;

        await _roleManager.UpdateAsync(role);

        return Unit.Value;
    }
}
