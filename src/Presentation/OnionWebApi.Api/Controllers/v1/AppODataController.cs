namespace OnionWebApi.Api.Controllers.v1;

[Route("odata")]
[ApiController]
[EnableQuery]
public class AppODataController(ISender sender) : ODataController
{
    public static IEdmModel GetEdmModel()
    {
        ODataConventionModelBuilder builder = new();
        builder.EnableLowerCamelCase();
        builder.EntitySet<GetAllBrandODataQueryResponse>("brands");
        return builder.GetEdmModel();
    }

    [HttpGet("brands")]
    public async Task<IQueryable<GetAllBrandODataQueryResponse>> GetAllBrands(CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetAllBrandODataQueryRequest(), cancellationToken);
        return response;
    }
}
