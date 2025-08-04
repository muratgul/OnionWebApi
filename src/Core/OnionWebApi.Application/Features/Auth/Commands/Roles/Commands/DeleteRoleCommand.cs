namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Commands;
public class DeleteRoleCommandRequest : IRequest<Unit>
{
    public string RoleName { get; set; }
}

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommandRequest, Unit>
{
    private readonly RoleManager<AppRole> _roleManager;

    public DeleteRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Unit> Handle(DeleteRoleCommandRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RoleName))
        {
            throw new ArgumentNullException(nameof(request.RoleName));
        }        

        await _roleManager.DeleteAsync(new() { Name = request.RoleName });

        return Unit.Value;
    }
}
