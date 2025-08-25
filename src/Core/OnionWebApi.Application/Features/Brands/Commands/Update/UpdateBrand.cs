using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Application.Features.Brands.Commands.Update;
public class UpdateBrandCommandRequest : IRequest<Brand>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
internal class UpdateBrandCommandHandler : BaseHandler, IRequestHandler<UpdateBrandCommandRequest, Brand>
{
    public UpdateBrandCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, IRedisCacheService redisCacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, redisCacheService)
    {
    }
    public async Task<Brand> Handle(UpdateBrandCommandRequest request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.GetReadRepository<Brand>().GetAsync(b => b.Id == request.Id) ?? throw new NotFoundException("Brand not found");

        brand.Name = request.Name;

        await _unitOfWork.GetWriteRepository<Brand>().UpdateAsync(brand);
        await _unitOfWork.SaveAsync();

        await _redisCacheService.RemoveAsync("GetAllBrands");

        return brand;
    }
}