namespace OnionWebApi.Application.Interfaces.Repositories;
public interface IReadRepository<T> where T : class, IEntityBase, new()
{
    IQueryable<T> GetAllQueryable(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool enableTracking = false);

    Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool enableTracking = false, CancellationToken cancellationToken = default);

    Task<PagedList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool enableTracking = false, int currentPage = 1, int pageSize = 3, CancellationToken cancellationToken = default);

    Task<T> GetAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool enableTracking = false, CancellationToken cancellationToken = default);

    IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false);

    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    PagingResult<T> GetListForPaging(int page, string propertyName, bool asc,
       Expression<Func<T, bool>>? expression = null, params Expression<Func<T, object>>[]? includeEntities);

    Task<IQueryable<T>> GetAllQueryable(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
