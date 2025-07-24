using UnionWebApi.Application.Utilities.Results;

namespace UnionWebApi.Application.Features.Brands.Quaries;
public class GetAllBrandsQueryResponse
{
    public required string Name { get; set; }
    public bool IsDeleted
    {
        get; set;
    }
}


public class GetAllBrandsQueryRequest : IRequest<IDataResult<IList<GetAllBrandsQueryResponse>>>, ICacheableQuery
{
    public string CacheKey => "GetAllBrands";
    public double CacheTime => 5;
}

public class GetAllBrandsQueryHandler : BaseHandler, IRequestHandler<GetAllBrandsQueryRequest, IDataResult<IList<GetAllBrandsQueryResponse>>>
{
    public GetAllBrandsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
    {
    }

    public async Task<IDataResult<IList<GetAllBrandsQueryResponse>>> Handle(GetAllBrandsQueryRequest request, CancellationToken cancellationToken)
    {
        var brands = await _unitOfWork.GetReadRepository<Brand>().GetAllAsync();

        var mapperData = _mapper.Map<GetAllBrandsQueryResponse, Brand>(brands);

        var result = new SuccessDataResult<IList<GetAllBrandsQueryResponse>>(mapperData, "Brands retrieved successfully.");

        return result;
    }
}