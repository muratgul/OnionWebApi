namespace OnionWebApi.Application.Features.Brands.Odata;

public class GetAllBrandODataQueryResponse : BrandDto
{
}

public sealed class GetAllBrandODataQueryRequest : IRequest<IQueryable<GetAllBrandODataQueryResponse>> { }

internal class GetAllBrandODataQueryHandler : BaseHandler, IRequestHandler<GetAllBrandODataQueryRequest, IQueryable<GetAllBrandODataQueryResponse>>
{
    public GetAllBrandODataQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
    }

    public Task<IQueryable<GetAllBrandODataQueryResponse>> Handle(GetAllBrandODataQueryRequest request, CancellationToken cancellationToken)
    {
        var brands = _unitOfWork.GetReadRepository<Brand>().GetAllQueryable().Result;

        var response = brands.Select(brand => new GetAllBrandODataQueryResponse
        {
            CreatedDate = brand.CreatedDate,
            Id = brand.Id,
            Name = brand.Name,
            NameId = brand.Name + " " + brand.Id,
            UpdatedDate = brand.UpdatedDate,
        });

        return Task.FromResult(response);
    }
}
