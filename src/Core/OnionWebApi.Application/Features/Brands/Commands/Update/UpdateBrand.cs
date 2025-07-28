namespace OnionWebApi.Application.Features.Brands.Commands.Update;
public class UpdateBrandCommandRequest : IRequest<Brand>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}


public class UpdateBrandCommandHandler : BaseHandler, IRequestHandler<UpdateBrandCommandRequest, Brand>
{
    public UpdateBrandCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService) : base(mapper, unitOfWork, httpContextAccessor, uriService)
    {
    }
    public async Task<Brand> Handle(UpdateBrandCommandRequest request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.GetReadRepository<Brand>().GetAsync(b => b.Id == request.Id);

        if (brand == null)
            throw new NotFoundException();

        brand.Name = request.Name;

        await _unitOfWork.GetWriteRepository<Brand>().UpdateAsync(brand);
        await _unitOfWork.SaveAsync();
        return brand;
    }
}