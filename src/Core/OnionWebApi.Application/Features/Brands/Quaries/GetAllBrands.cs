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
    public string CacheKey => "GetAllBrands";
    [JsonIgnore]
    public double CacheTime => 5;
}

internal class GetAllBrandsQueryHandler : BaseHandler, IRequestHandler<GetAllBrandsQueryRequest, PaginatedResult<IEnumerable<GetAllBrandsQueryResponse>>>
{
    private readonly IPaginationService _paginationService;
    private readonly IMassTransitSend _massTransitSend;
    public GetAllBrandsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService, IPaginationService paginationService, IMassTransitSend massTransitSend) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
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
            Include = q => q.Include(e => e.CreatedUser).Include(e => e.UpdatedUser).Include(e => e.DeletedUser),
            OrderBy = null,
            EnableTracking = false,
            Route = "Brands/GetAll",
            Fields = null
        };

        return await _paginationService.GetPaginatedDataAsync<Brand, GetAllBrandsQueryResponse>(paginationRequest, cancellationToken);        
    }
}