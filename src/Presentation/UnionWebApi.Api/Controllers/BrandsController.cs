using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnionWebApi.Application.Features.Brands.Quaries;

namespace UnionWebApi.Api.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class BrandsController : ControllerBase
{
    private readonly IMediator mediator;

    public BrandsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var response = await mediator.Send(new GetAllBrandsQueryRequest());
        return Ok(response);
    }
}
