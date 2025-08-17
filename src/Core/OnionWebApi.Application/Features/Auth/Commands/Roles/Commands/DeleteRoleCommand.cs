namespace OnionWebApi.Application.Features.Auth.Commands.Roles.Commands;
public class DeleteRoleCommandRequest : IRequest
{
    public string RoleName { get; set; }
}

internal class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommandRequest>
{
    private readonly RoleManager<AppRole> _roleManager;

    public DeleteRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task Handle(DeleteRoleCommandRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RoleName))
        {
            throw new ArgumentNullException(nameof(request.RoleName));
        }        

        var result = await _roleManager.DeleteAsync(new() { Name = request.RoleName });

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.Select(x => x.Description).FirstOrDefault());
        }

        
    }
}
