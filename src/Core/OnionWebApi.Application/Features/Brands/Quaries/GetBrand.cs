namespace OnionWebApi.Application.Features.Brands.Quaries;
public class GetBrandQueryResponse : BrandDto
{
}

public class GetBrandQueryRequest : IRequest<IDataResult<GetBrandQueryResponse>>
{
    public int Id { get; set; }
}

public class GetBrandQueryHandler : BaseHandler, IRequestHandler<GetBrandQueryRequest, IDataResult<GetBrandQueryResponse>>
{
    public GetBrandQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
    }

    public async Task<IDataResult<GetBrandQueryResponse>> Handle(GetBrandQueryRequest request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.GetReadRepository<Brand>().GetAsync(x => x.Id == request.Id);
        
        if (brand == null)
        {
            return new ErrorDataResult<GetBrandQueryResponse>("Brand not found");            
        }

        var mapperBrand = brand.Adapt<GetBrandQueryResponse>();

        var result = new SuccessDataResult<GetBrandQueryResponse>(mapperBrand, "Data is listed");

        return result;
    }
}