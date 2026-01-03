namespace OnionWebApi.Application.Features.Brands.Commands.Create;
public class CreateBrandCommandRequest : IRequest<IDataResult<Brand>>
{
    public string Name { get; set; } = default!;
}

internal class CreateBrandCommandHandler(IMapper mapper, BrandRules brandRules, 
    IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService) : 
    BaseHandler(mapper, unitOfWork, httpContextAccessor, uriService, cacheService), IRequestHandler<CreateBrandCommandRequest, IDataResult<Brand>>
{
    private readonly BrandRules _brandRules = brandRules;

    public async Task<IDataResult<Brand>> Handle(CreateBrandCommandRequest request, CancellationToken cancellationToken)
    {

        var existingBrand = await _unitOfWork.GetReadRepository<Brand>().GetAllAsync(b => b.Name.ToLower() == request.Name.ToLower() && !b.IsDeleted);

        if (existingBrand != null) 
        {
            throw new BrandAlreadyExistsException();
        }

        await _brandRules.BrandNameCheck(request.Name);

        var brand = _mapper.Map<Brand>(request);
        brand.IsDeleted = false;

        brand.CreateBrand(request.Name);

        await _unitOfWork.GetWriteRepository<Brand>().AddAsync(brand, cancellationToken);
        await _unitOfWork.SaveAsync();

        
        await _cacheService.RemoveByTagAsync("GetAllBrands", cancellationToken);        

        return new SuccessDataResult<Brand>(brand);
    }
}
