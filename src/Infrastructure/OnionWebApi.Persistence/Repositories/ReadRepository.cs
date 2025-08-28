namespace OnionWebApi.Persistence.Repositories;
public class ReadRepository<T>(DbContext dbContext) : IReadRepository<T> where T : class, IEntityBase, new()
{
    private readonly DbContext dbContext = dbContext;
    private DbSet<T> Table => dbContext.Set<T>();


    public IQueryable<T> GetAllQueryable(Expression<Func<T, bool>>? predicate = null, 
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
        bool enableTracking = false)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include is not null)
        {
            queryable = include(queryable);
        }

        if (predicate is not null)
        {
            queryable = queryable.Where(predicate);
        }

        return orderBy is not null ? orderBy(queryable) : queryable;
    }


    public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, 
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
        bool enableTracking = false, 
        CancellationToken cancellationToken = default)
    {
        var queryable = GetAllQueryable(predicate, include, orderBy, enableTracking);
        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<PagedList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracking = false, int currentPage = 1, int pageSize = 3, CancellationToken token = default)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include is not null)
        {
            queryable = include(queryable);
        }

        if (predicate is not null)
        {
            queryable = queryable.Where(predicate);
        }

        return orderBy is not null
            ? await PagedList<T>.CreateAsync(orderBy(queryable), currentPage, pageSize)
            : await PagedList<T>.CreateAsync(queryable, currentPage, pageSize);
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, 
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, 
        bool enableTracking = false, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include is not null)
        {
            queryable = include(queryable);
        }

        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, 
        CancellationToken cancellationToken = default)
    {
        var queryable = Table.AsNoTracking();
        return predicate == null 
            ? await queryable.CountAsync(cancellationToken) 
            : await queryable.CountAsync(predicate, cancellationToken);
    }

    public IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false)
    {
        var queryable = Table.AsQueryable();
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        return queryable.Where(predicate);
    }

    public PagingResult<T> GetListForPaging(int page, 
        string propertyName, 
        bool asc, 
        Expression<Func<T, bool>>? expression = null, 
        params Expression<Func<T, object>>[]? includeEntities)
    {
        var list = Table.AsNoTracking().AsQueryable();

        if (includeEntities?.Length > 0)
        {
            list = list.IncludeMultiple(includeEntities);
        }

        if (expression is not null)
        {
            list = list.Where(expression);
        }

        list = asc 
            ? list.OrderByDynamic(propertyName, ESort.ASC) 
            : list.OrderByDynamic(propertyName, ESort.DESC);
        
        var totalCount = list.Count();

        if (totalCount == 0)
        {
            return new PagingResult<T>(new List<T>(), 0, true, "No record found");
        }

        const int pageSize = 0;
        var start = (page - 1) * pageSize;
        var pagedData = list.Skip(start).Take(pageSize).ToList();

        return new PagingResult<T>(pagedData, totalCount, true, $"{totalCount} record(s) found, showing page {page}");
    }
    public Task<IQueryable<T>> GetAllQueryable(CancellationToken token = default)
    {
        IQueryable<T> queryable = Table;

        return Task.FromResult(queryable);
    }    
}
