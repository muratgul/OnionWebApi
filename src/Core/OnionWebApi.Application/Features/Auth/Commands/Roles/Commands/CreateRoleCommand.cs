namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Commands;
public class CreateRoleCommandRequest : IRequest<Unit>
{
    public string RoleName { get; set; }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommandRequest, Unit>
{
    private readonly RoleManager<AppRole> _roleManager;
    public CreateRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Unit> Handle(CreateRoleCommandRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
        {
            throw new ArgumentNullException(nameof(request.RoleName));
        }

        await _roleManager.CreateAsync(new() { Name = request.RoleName });

        return Unit.Value;
    }
}
