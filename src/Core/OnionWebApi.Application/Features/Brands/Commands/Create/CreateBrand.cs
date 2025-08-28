using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Application.Features.Brands.Commands.Create;
public class CreateBrandCommandRequest : IRequest<Brand>
{
    public string Name { get; set; } = default!;
}

internal class CreateBrandCommandHandler(BrandRules brandRules, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService) : BaseHandler(null, unitOfWork, httpContextAccessor, uriService, cacheService), IRequestHandler<CreateBrandCommandRequest, Brand>
{
    private readonly BrandRules _brandRules = brandRules;

    public async Task<Brand> Handle(CreateBrandCommandRequest request, CancellationToken cancellationToken)
    {
        await _brandRules.BrandNameCheck(request.Name);

        var brand = _mapper.Map<Brand>(request);
        brand.IsDeleted = false;

        brand.CreateBrand(request.Name);

        await _unitOfWork.GetWriteRepository<Brand>().AddAsync(brand, cancellationToken);
        await _unitOfWork.SaveAsync();

        await _cacheService.RemoveAsync("GetAllBrands", cancellationToken);

        return brand;
    }
}
