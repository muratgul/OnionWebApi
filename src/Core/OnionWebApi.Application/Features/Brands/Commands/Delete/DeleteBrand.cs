using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Application.Features.Brands.Commands.Delete;
public class DeleteBrandCommandRequest : IRequest<Unit>
{
    public int Id { get; set;}
}


internal class DeleteBrandCommandHandler : BaseHandler, IRequestHandler<DeleteBrandCommandRequest, Unit>
{
    public DeleteBrandCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, cacheService)
    {
    }

    public async Task<Unit> Handle(DeleteBrandCommandRequest request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.GetReadRepository<Brand>().GetAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken: cancellationToken) ?? throw new BrandNotFoundException();

        brand.IsDeleted = true;

        await _unitOfWork.GetWriteRepository<Brand>().SoftDeleteAsync(brand, cancellationToken);
        await _unitOfWork.SaveAsync();

        await _cacheService.RemoveAsync("GetAllBrands", cancellationToken);

        return Unit.Value;

                
    }
}