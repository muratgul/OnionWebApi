using OnionWebApi.Application.Rules;

namespace OnionWebApi.Application.Features.Brands.Commands.Create;
public class CreateBrandCommandRequest : IRequest<Brand>
{
    public string Name { get; set; } = default!;
}

public class CreateBrandCommandHandler : BaseHandler, IRequestHandler<CreateBrandCommandRequest, Brand>
{
    private readonly BrandRules _brandRules;
    public CreateBrandCommandHandler(BrandRules brandRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {        
        _brandRules = brandRules;
    }

    public async Task<Brand> Handle(CreateBrandCommandRequest request, CancellationToken cancellationToken)
    {
        await _brandRules.BrandNameCheck(request.Name);

        var brand = _mapper.Map<Brand, CreateBrandCommandRequest>(request);
        brand.IsDeleted = false;

        await _unitOfWork.GetWriteRepository<Brand>().AddAsync(brand);
        await _unitOfWork.SaveAsync();

        await _redisCacheService.RemoveAsync("GetAllBrands");

        return brand;
    }
}
