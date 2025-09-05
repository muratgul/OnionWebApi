namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Commands;
public record CreateRoleCommandRequest(string RoleName) : IRequest<Unit>;

internal class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommandRequest, Unit>
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

        var result = await _roleManager.CreateAsync(new() { Name = request.RoleName });

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.Select(x => x.Description).FirstOrDefault());
        }

        return Unit.Value;
    }
}
