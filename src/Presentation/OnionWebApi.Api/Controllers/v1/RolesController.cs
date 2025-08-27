namespace OnionWebApi.Api.Controllers.v1;

public class RolesController : BaseController
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await Mediator.Send(new GetAllRolesQueryRequest()));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleCommandRequest request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateRoleCommandRequest request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteRoleCommandRequest request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AddUserRole(CreateUserRoleCommandRequest request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> DeleteUserRole(DeleteUserRoleCommandRequest request)
    {
        await Mediator.Send(request);
        return Ok();
    }
}
