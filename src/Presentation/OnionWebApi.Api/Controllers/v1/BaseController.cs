namespace OnionWebApi.Api.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public abstract class BaseController : ControllerBase
{
    private ISender? _mediator;
    private IMassTransitSend _massTransitSend;
    private INotificationService _notificationService;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    protected IMassTransitSend MassTransitSend => _massTransitSend ??= HttpContext.RequestServices.GetRequiredService<IMassTransitSend>();
    protected INotificationService NotificationService => _notificationService ??= HttpContext.RequestServices.GetRequiredService<INotificationService>();


}
