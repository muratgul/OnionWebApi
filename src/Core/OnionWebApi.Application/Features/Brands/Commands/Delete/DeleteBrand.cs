namespace OnionWebApi.Application.Features.Brands.Commands.Delete;
public class DeleteBrandCommandRequest : IRequest<IDataResult<Unit>>
{
    public int Id { get; set;}
}


internal class DeleteBrandCommandHandler : BaseHandler, IRequestHandler<DeleteBrandCommandRequest, IDataResult<Unit>>
{
    public DeleteBrandCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUriService uriService, ICacheService cacheService) : base(mapper, unitOfWork, httpContextAccessor, uriService, cacheService)
    {
    }

    public async Task<IDataResult<Unit>> Handle(DeleteBrandCommandRequest request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.GetReadRepository<Brand>().GetAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken: cancellationToken) ?? throw new BrandNotFoundException();

        brand.IsDeleted = true;

        await _unitOfWork.GetWriteRepository<Brand>().SoftDeleteAsync(brand, cancellationToken);
        await _unitOfWork.SaveAsync();

        await _cacheService.RemoveAsync("GetAllBrands", cancellationToken);

        return new SuccessDataResult<Unit>();                
    }
}