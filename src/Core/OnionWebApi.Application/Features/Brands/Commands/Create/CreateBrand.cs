namespace OnionWebApi.Application.Features.Brands.Commands.Create;
public class CreateBrandCommandRequest : IRequest<Brand>
{
    public string Name { get; set; } = default!;
}

public class CreateBrandCommandHandler : BaseHandler, IRequestHandler<CreateBrandCommandRequest, Brand>
{
    public CreateBrandCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService) : base(mapper, unitOfWork, httpContextAccessor, uriService)
    {
    }

    public async Task<Brand> Handle(CreateBrandCommandRequest request, CancellationToken cancellationToken)
    {   
        var brand = _mapper.Map<Brand, CreateBrandCommandRequest>(request);
        brand.IsDeleted = false;

        await _unitOfWork.GetWriteRepository<Brand>().AddAsync(brand);
        await _unitOfWork.SaveAsync();       

        return brand;
    }
}
