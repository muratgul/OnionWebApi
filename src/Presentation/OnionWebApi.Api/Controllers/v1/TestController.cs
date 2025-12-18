using MG.Results;
using OnionWebApi.Domain.Entities;

namespace OnionWebApi.Api.Controllers.v1;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
       var brand = new Brand
        {
            Name = "Test Brand",
        };

        var res = Result.Success<Brand>(brand);

        return Ok(res);
    }
}
