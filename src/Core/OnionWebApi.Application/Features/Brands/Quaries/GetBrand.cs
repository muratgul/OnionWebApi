using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Application.Features.Brands.Quaries;
public class GetBrandQueryResponse : BrandDto
{
    public string CreatedUserName { get; set; } = default!;
    public string? UpdatedUserName { get; set; }
    public string? DeletedUserName { get; set; }
}

public class GetBrandQueryRequest : IRequest<IDataResult<GetBrandQueryResponse>>
{
    public int Id { get; set; }
}

internal class GetBrandQueryHandler : BaseHandler, IRequestHandler<GetBrandQueryRequest, IDataResult<GetBrandQueryResponse>>
{
    public GetBrandQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
    }

    public async Task<IDataResult<GetBrandQueryResponse>> Handle(GetBrandQueryRequest request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.GetReadRepository<Brand>().GetAsync(
            predicate: x => x.Id == request.Id,
            include: q => q.Include(e => e.CreatedUser).Include(e => e.UpdatedUser).Include(e => e.DeletedUser));

        if (brand == null)
        {
            return new ErrorDataResult<GetBrandQueryResponse>("Brand not found");            
        }

        var mapperBrand = brand.Adapt<GetBrandQueryResponse>();

        var result = new SuccessDataResult<GetBrandQueryResponse>(mapperBrand, "Data is listed");

        return result;
    }
}