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
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommandRequest request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var response = await Mediator.Send(new GetAllUsersQueryRequest());
        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginCommandRequest request)
    {
        var response = await Mediator.Send(request);
        return StatusCode(StatusCodes.Status200OK, response);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommandRequest request)
    {
        await Mediator.Send(request);
        return StatusCode(StatusCodes.Status200OK);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePasswordUsingToken(ChangePasswordUsingTokenCommandRequest request)
    {
        await Mediator.Send(request);
        return StatusCode(StatusCodes.Status200OK);
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

    [HttpGet]
    public IActionResult OtpTest()
    {
        var result = _otpService.CreateOtpSetup("OnionWebApi", "muratgul");

        return Ok(result);
    }
}
