using OnionWebApi.Application.Services;
using OnionWebApi.Application.Wrappers;

namespace OnionWebApi.Application.Features.Brands.Quaries;
public class GetAllBrandsQueryResponse : BrandDto
{
}

public class GetAllBrandsQueryRequest : PagingParameter, IRequest<PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>>, ICacheableQuery
{
    public string CacheKey => "GetAllBrands";
    public double CacheTime => 5;
}

public class GetAllBrandsQueryHandler : BaseHandler, IRequestHandler<GetAllBrandsQueryRequest, PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>>
{
    private readonly IPaginationService _paginationService;
    public GetAllBrandsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService, IPaginationService paginationService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _paginationService = paginationService;
    }

    public async Task<PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>> Handle(GetAllBrandsQueryRequest request, CancellationToken cancellationToken)
    {
        var paginationRequest = new PaginationRequest<Brand>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Predicate = null,
            Include = null,
            OrderBy = null,
            EnableTracking = false,
            Route = "Brands/GetAll",
            Fields = null
        };

        return await _paginationService.GetPaginatedDataAsync<Brand, GetAllBrandsQueryResponse>(paginationRequest, cancellationToken);

        //var pagedBrands = await _unitOfWork.GetReadRepository<Brand>().GetAllByPagingAsync(
        // predicate: null,
        // include: null,
        // orderBy: null,
        // enableTracking: false,
        // currentPage: request.PageNumber,
        // pageSize: request.PageSize);

        //var mappedData = _mapper.Map<GetAllBrandsQueryResponse, Brand>(pagedBrands.Items);

        //var result = PaginationHelper.CreatePaginatedResponse(
        //   isDynamic: false,
        //   data: mappedData,
        //   paginationFilter: new PaginationFilter(request.PageNumber, request.PageSize),
        //   totalRecords: pagedBrands.TotalCount,
        //   uriService: _uriService,
        //   route: "Brands/GetAll",
        //   fields: null);

        //return result;
    }
}