namespace OnionWebApi.Api.Controllers;

[Route("api/[controller]/[action]")]
public class AuthController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Register(RegisterCommandRequest request)
    {
        await Mediator.Send(request);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginCommandRequest request)
    {
        var response = await Mediator.Send(request);
        return StatusCode(StatusCodes.Status200OK, response);
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommandRequest request)
    {
        var response = await Mediator.Send(request);
        return StatusCode(StatusCodes.Status200OK, response);
    }

    [HttpPost]
    public async Task<IActionResult> Revoke(RevokeCommandRequest request)
    {
        await Mediator.Send(request);
        return StatusCode(StatusCodes.Status200OK);
    }

    [HttpPost]
    public async Task<IActionResult> RevokeAll()
    {
        await Mediator.Send(new RevokeAllCommandRequest());
        return StatusCode(StatusCodes.Status200OK);
    }
}
