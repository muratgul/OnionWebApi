using OnionWebApi.Application.Features.Brands.Commands.Create;
using OnionWebApi.Application.Features.Brands.Commands.Delete;
using OnionWebApi.Application.Features.Brands.Commands.Update;

namespace OnionWebApi.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllBrandsQueryRequest request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateBrandCommandRequest request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateBrandCommandRequest request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteBrandCommandRequest request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetBrandQueryRequest request)
    {
        return Ok(await _mediator.Send(request));
    }
}
