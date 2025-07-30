namespace OnionWebApi.Application.Services;

public interface IPaginationService
{
    Task<PaginatedResult<IEnumerable<TDto>>> GetPaginatedDataAsync<TEntity, TDto>(
        PaginationRequest<TEntity> request,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntityBase, new()
        where TDto : class, new();
}

