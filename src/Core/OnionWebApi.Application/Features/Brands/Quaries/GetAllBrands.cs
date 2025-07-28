namespace OnionWebApi.Application.Features.Brands.Quaries;
public class GetAllBrandsQueryResponse : Brand
{
    //public required string Name { get; set; }
    //public bool IsDeleted { get; set; }
}


public class GetAllBrandsQueryRequest : PagingParameter, IRequest<PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>>, ICacheableQuery
{
    public string CacheKey => "GetAllBrands";
    public double CacheTime => 5;
}

public class GetAllBrandsQueryHandler : BaseHandler, IRequestHandler<GetAllBrandsQueryRequest, PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>>
{
    public GetAllBrandsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService) : base(mapper, unitOfWork, httpContextAccessor, uriService)
    {
    }

    public async Task<PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>> Handle(GetAllBrandsQueryRequest request, CancellationToken cancellationToken)
    {
        Log.Information("Voodoo");

        var pagedBrands = await _unitOfWork.GetReadRepository<Brand>().GetAllByPagingAsync(
         predicate: null,
         include: null,
         orderBy: null,
         enableTracking: false,
         currentPage: request.PageNumber,
         pageSize: request.PageSize);

        var mappedData = _mapper.Map<GetAllBrandsQueryResponse, Brand>(pagedBrands.Items);

        return PaginationHelper.CreatePaginatedResponse(
           isDynamic: false,
           data: mappedData,
           paginationFilter: new PaginationFilter(request.PageNumber, request.PageSize),
           totalRecords: pagedBrands.TotalCount,
           uriService: _uriService,
           route: "Brands/GetAll",
           fields: null);
    }
}