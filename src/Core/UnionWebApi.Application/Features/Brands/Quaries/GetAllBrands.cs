namespace UnionWebApi.Application.Features.Brands.Quaries;
public class GetAllBrandsQueryResponse
{
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
}


public class GetAllBrandsQueryRequest : IRequest<IList<GetAllBrandsQueryResponse>>, ICacheableQuery
{
    public string CacheKey => "GetAllBrands";
    public double CacheTime => 5;
}

public class GetAllBrandsQueryHandler : BaseHandler, IRequestHandler<GetAllBrandsQueryRequest, IList<GetAllBrandsQueryResponse>>
{
    public GetAllBrandsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
    {
    }

    public async Task<IList<GetAllBrandsQueryResponse>> Handle(GetAllBrandsQueryRequest request, CancellationToken cancellationToken)
    {
        var brands = await _unitOfWork.GetReadRepository<Brand>().GetAllAsync();

        return _mapper.Map<GetAllBrandsQueryResponse, Brand>(brands);
    }
}