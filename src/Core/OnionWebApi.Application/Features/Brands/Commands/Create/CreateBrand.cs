namespace OnionWebApi.Application.Features.Brands.Commands.Create;
public class CreateBrandCommandRequest : IRequest<Brand>
{
    public string Name { get; set; } = default!;
}

internal class CreateBrandCommandHandler : BaseHandler, IRequestHandler<CreateBrandCommandRequest, Brand>
{
    private readonly BrandRules _brandRules;
    private readonly IMediator _mediator;
    public CreateBrandCommandHandler(BrandRules brandRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService, IMediator mediator) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {        
        _brandRules = brandRules;
        _mediator = mediator;
    }

    public async Task<Brand> Handle(CreateBrandCommandRequest request, CancellationToken cancellationToken)
    {
        await _brandRules.BrandNameCheck(request.Name);

        var brand = _mapper.Map<Brand>(request);
        brand.IsDeleted = false;

        await _unitOfWork.GetWriteRepository<Brand>().AddAsync(brand);
        await _unitOfWork.SaveAsync();

        brand.CreateBrand(request.Name);

        foreach (var domainEvent in brand.DomainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        brand.ClearDomainEvents();

        await _redisCacheService.RemoveAsync("GetAllBrands");

        return brand;
    }
}
