namespace OnionWebApi.Application.Features.Brands.Commands.Update;
public class UpdateBrandCommandRequest : IRequest<IDataResult<Brand>>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
internal class UpdateBrandCommandHandler : BaseHandler, IRequestHandler<UpdateBrandCommandRequest, IDataResult<Brand>>
{
    public UpdateBrandCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, cacheService)
    {
    }
    public async Task<IDataResult<Brand>> Handle(UpdateBrandCommandRequest request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.GetReadRepository<Brand>().GetAsync(b => b.Id == request.Id, cancellationToken: cancellationToken) ?? throw new BrandNotFoundException();

        if (brand.Name == request.Name)
        {
            return new SuccessDataResult<Brand>(brand);
        }

        
        brand.Name = request.Name;

        await _unitOfWork.GetWriteRepository<Brand>().UpdateAsync(brand, cancellationToken);
        await _unitOfWork.SaveAsync();

        await _cacheService.RemoveByTagAsync("GetAllBrands", cancellationToken);

        return new SuccessDataResult<Brand>(brand);
    }
}