namespace OnionWebApi.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public abstract class BaseController : ControllerBase
{
    private Sender _mediator;
    protected Sender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<Sender>();
}
