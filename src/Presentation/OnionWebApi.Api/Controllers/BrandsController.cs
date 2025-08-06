namespace OnionWebApi.Api.Controllers;

//[Authorize]
public class BrandsController : BaseController
{
    private readonly IMassTransitSend _massTransitSend;
   
    public BrandsController(IMassTransitSend massTransitSend)
    {
        _massTransitSend = massTransitSend;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllBrandsQueryRequest request)
    {
        return Ok(await Mediator.Send(request));
    }

    [Idempotent]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateBrandCommandRequest request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateBrandCommandRequest request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteBrandCommandRequest request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetBrandQueryRequest request)
    {
        return Ok(await Mediator.Send(request));
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
