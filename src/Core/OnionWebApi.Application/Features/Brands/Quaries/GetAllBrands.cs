
namespace OnionWebApi.Application.Features.Brands.Quaries;
public class GetAllBrandsQueryResponse : BrandDto
{
    public string CreatedUserName { get; set; } = default!;
    public string? UpdatedUserName { get; set; }
    public string? DeletedUserName { get; set; }
}

public class GetAllBrandsQueryRequest : PagingParameter, IRequest<PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>>, ICacheableQuery
{
    [JsonIgnore]
    public string CacheKey => $"GetAllBrands_P{PageNumber}_S{PageSize}";
    [JsonIgnore]
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
    [JsonIgnore]
    public IEnumerable<string>? CacheTags => ["GetAllBrands"];
}

internal class GetAllBrandsQueryHandler : BaseHandler, IRequestHandler<GetAllBrandsQueryRequest, PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>>
{
    private readonly IPaginationService _paginationService;
    private readonly IMassTransitSend _massTransitSend;
    public GetAllBrandsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService, IPaginationService paginationService, IMassTransitSend massTransitSend) : base(mapper, unitOfWork, httpContextAccessor, uriService, cacheService)
    {
        _paginationService = paginationService;
        _massTransitSend = massTransitSend;
    }

    public async Task<PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>> Handle(GetAllBrandsQueryRequest request, CancellationToken cancellationToken)
    {
        await _massTransitSend.SendToQueue(message: "Buraya bilgi gelecek", queueName: "xxx", cancellationToken: default);

        var paginationRequest = new PaginationRequest<Brand>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Predicate = null,
            Include = q => q.Include(e => e.CreatedUser).Include(e => e.UpdatedUser).Include(e => e.DeletedUser)!,
            OrderBy = null,
            EnableTracking = false,
            Route = "Brands/GetAll",
            Fields = null
        };

        var result = await _paginationService.GetPaginatedDataAsync<Brand, GetAllBrandsQueryResponse>(paginationRequest, cancellationToken);

        return result;        
    }
}