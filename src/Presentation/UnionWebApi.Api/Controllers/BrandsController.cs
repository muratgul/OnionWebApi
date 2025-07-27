using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using UnionWebApi.Application.Features.Brands.Commands;
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
    public async Task<IActionResult> GetAll([FromQuery] GetAllBrandsQueryRequest request)
    {
        Log.Information("Handling GetAllBrandsQueryRequest");
        var response = await mediator.Send(request);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateBrandCommandRequest request)
    {
        var response = await mediator.Send(request);
        return Ok(response);
    }
}
