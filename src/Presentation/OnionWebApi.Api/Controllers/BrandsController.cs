namespace OnionWebApi.Api.Controllers;
public class BrandsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllBrandsQueryRequest request)
    {
        return Ok(await Mediator.Send(request));
    }

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
}
