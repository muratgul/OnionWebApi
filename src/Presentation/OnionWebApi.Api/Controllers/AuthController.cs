using OnionWebApi.Application.Interfaces.Otp;

namespace OnionWebApi.Api.Controllers;

[Route("api/[controller]/[action]")]
public class AuthController : BaseController
{
    private readonly IOtpService _otpService;
    public AuthController(IOtpService otpService)
    {
        _otpService = otpService;
    }

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

    [HttpGet("OtpTest")]
    public IActionResult OtpTest()
    {
        var result = _otpService.FirstCode("OnionWebApi", "muratgul");

        return Ok(result);
    }
}
