namespace OnionWebApi.Application.Services;
public class PaginationService : IPaginationService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUriService _uriService;

    public PaginationService(IMapper mapper, IUnitOfWork unitOfWork, IUriService uriService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _uriService = uriService;
    }

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

        var mappedData = _mapper.Map<TDto, TEntity>(pagedData.Items);

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
