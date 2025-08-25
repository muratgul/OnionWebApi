using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Application.Features.Brands.Commands.Create;
public class CreateBrandCommandRequest : IRequest<Brand>
{
    public string Name { get; set; } = default!;
}

internal class CreateBrandCommandHandler : BaseHandler, IRequestHandler<CreateBrandCommandRequest, Brand>
{
    private readonly BrandRules _brandRules;
    public CreateBrandCommandHandler(BrandRules brandRules, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(null, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
        _brandRules = brandRules;
    }

    public async Task<Brand> Handle(CreateBrandCommandRequest request, CancellationToken cancellationToken)
    {
        await _brandRules.BrandNameCheck(request.Name);

        var brand = _mapper.Map<Brand>(request);
        brand.IsDeleted = false;

        brand.CreateBrand(request.Name);

        await _unitOfWork.GetWriteRepository<Brand>().AddAsync(brand);
        await _unitOfWork.SaveAsync();

        await _redisCacheService.RemoveAsync("GetAllBrands");

        return brand;
    }
}
