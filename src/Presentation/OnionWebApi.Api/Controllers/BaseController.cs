namespace OnionWebApi.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    private IMediator? _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
