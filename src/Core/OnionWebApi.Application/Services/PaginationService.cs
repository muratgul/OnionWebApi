namespace OnionWebApi.Application.Services;
public class PaginationService(IMapper mapper, IUnitOfWork unitOfWork, IUriService uriService) : IPaginationService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUriService _uriService = uriService;

    public async Task<PaginatedResult<IEnumerable<TDto>>> GetPaginatedDataAsync<TEntity, TDto>(
            PaginationRequest<TEntity> request,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntityBase, new()
            where TDto : class, new()
    {
        var pagedData = await _unitOfWork.GetReadRepository<TEntity>().GetAllByPagingAsync(
            predicate: request.Predicate,
            include: request.Include,
            orderBy: request.OrderBy,
            enableTracking: request.EnableTracking,
            currentPage: request.PageNumber,
            pageSize: request.PageSize);

        var mappedData = _mapper.Map<IEnumerable<TDto>>(pagedData.Items);

        return PaginationHelper.CreatePaginatedResponse(
                isDynamic: false,
                data: mappedData,
                paginationFilter: new PaginationFilter(request.PageNumber, request.PageSize),
                totalRecords: pagedData.TotalCount,
                uriService: _uriService,
                route: request.Route,
                fields: request.Fields);
    }
}
