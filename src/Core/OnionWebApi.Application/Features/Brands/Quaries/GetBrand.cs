namespace OnionWebApi.Application.Features.Brands.Quaries;
public class GetBrandQueryResponse : BrandDto
{
    public string CreatedUserName { get; set; } = default!;
    public string? UpdatedUserName { get; set; }
    public string? DeletedUserName { get; set; }
}

public class GetBrandQueryRequest : IRequest<DataResult<GetBrandQueryResponse>>, ICacheableQuery
{
    [JsonIgnore]
    public string CacheKey => $"GetBrand_{Id}";
    [JsonIgnore]
    public double CacheTime => 5;
    public int Id { get; set; }
}

internal class GetBrandQueryHandler : BaseHandler, IRequestHandler<GetBrandQueryRequest, DataResult<GetBrandQueryResponse>>
{
    public GetBrandQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, cacheService)
    {
    }

    public async Task<DataResult<GetBrandQueryResponse>> Handle(GetBrandQueryRequest request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.GetReadRepository<Brand>().GetAsync(predicate: x => x.Id == request.Id, include: q => q.Include(e => e.CreatedUser).Include(e => e.UpdatedUser).Include(e => e.DeletedUser)!, cancellationToken: cancellationToken);

        if (brand is null)
        {
            return new ErrorDataResult<GetBrandQueryResponse>("Brand not found");            
        }

        var mapperBrand = _mapper.Map<GetBrandQueryResponse>(brand); //brand.Adapt<GetBrandQueryResponse>();

        var result = new SuccessDataResult<GetBrandQueryResponse>(mapperBrand, "Data is listed");

        return result;
    }
}