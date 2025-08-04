using OnionWebApi.Application.Features.Auth.Commands.Roles.Commands;
using OnionWebApi.Application.Features.Auth.Commands.Roles.Quaries;

namespace OnionWebApi.Api.Controllers;

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
}
