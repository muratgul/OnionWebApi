namespace OnionWebApi.Api.Controllers.v1;

//[Authorize]
public class BrandsController : BaseController
{
    private readonly IMassTransitSend _massTransitSend;
    private readonly INotificationService _notificationService;
   
    public BrandsController(IMassTransitSend massTransitSend, INotificationService notificationService)
    {
        _massTransitSend = massTransitSend;
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllBrandsQueryRequest request, CancellationToken cancellationToken)
    {       
        return Ok(await Mediator.Send(request, cancellationToken));
    }
    
    [HttpPost]
    [Idempotent]
    public async Task<IActionResult> Add([FromBody] CreateBrandCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        await _notificationService.SendToAllAsync("ReceiveNotofication", new NotificationMessage
        {
            Type = "Info",
            Content = "Brand is created"
        });
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateBrandCommandRequest request, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(request, cancellationToken));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteBrandCommandRequest request, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(request, cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetBrandQueryRequest request, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(request, cancellationToken));
    }

    [HttpPost("PublishTestEvent")]
    public async Task<IActionResult> PublishTestEvent()
    {   
        var message = new MassTransitMessage<string>
        {
            Data = "Test event from BrandsController" + " at " + DateTime.UtcNow.ToString("o")
        };

        await _massTransitSend.SendToEndpoint("brand-message-queue", message, cancellationToken: default);
        await _massTransitSend.SendToQueue("Buraya bilgi gelecek","MuratQueue", cancellationToken: default);
        return Ok("Test event published to RabbitMQ.");
    }
}
