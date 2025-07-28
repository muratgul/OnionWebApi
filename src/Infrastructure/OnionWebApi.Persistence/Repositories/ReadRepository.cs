using OnionWebApi.Application.Enums;
using OnionWebApi.Application.Extensions;
using OnionWebApi.Application.Utilities.Results;

namespace OnionWebApi.Persistence.Repositories;
public class ReadRepository<T> : IReadRepository<T> where T : class, IEntityBase, new()
{
    private readonly DbContext dbContext;

    public ReadRepository(DbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    private DbSet<T> Table { get => dbContext.Set<T>(); }


    public IQueryable<T> GetAllQueryable(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracking = false)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include is not null)
            queryable = include(queryable);
        if (predicate is not null)
            queryable = queryable.Where(predicate);
        if (orderBy is not null)
            return orderBy(queryable);
        return queryable;
    }


    public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracking = false)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include is not null)
            queryable = include(queryable);
        if (predicate is not null)
            queryable = queryable.Where(predicate);
        if (orderBy is not null)
            return await orderBy(queryable).ToListAsync();

        return await queryable.ToListAsync();
    }

    public async Task<PagedList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracking = false, int currentPage = 1, int pageSize = 3)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include is not null)
            queryable = include(queryable);
        if (predicate is not null)
            queryable = queryable.Where(predicate);
        if (orderBy is not null)
            return await PagedList<T>.CreateAsync(orderBy(queryable), currentPage, pageSize);

        return await PagedList<T>.CreateAsync(queryable, currentPage, pageSize);
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool enableTracking = false)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include is not null)
            queryable = include(queryable);

        //queryable.Where(predicate);

        return await queryable.FirstOrDefaultAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        Table.AsNoTracking();
        if (predicate is not null)
            Table.Where(predicate);

        return await Table.CountAsync();
    }

    public IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false)
    {
        if (!enableTracking)
            Table.AsNoTracking();
        return Table.Where(predicate);
    }

    public PagingResult<T> GetListForPaging(int page, string propertyName, bool asc, Expression<Func<T, bool>>? expression = null, params Expression<Func<T, object>>[]? includeEntities)
    {
        var list = Table.AsQueryable();

        if (includeEntities.Length > 0)
            list = list.IncludeMultiple(includeEntities);

        if (expression != null)
            list = list.Where(expression).AsQueryable();

        list = asc ? list.AscOrDescOrder(ESort.ASC, propertyName) : list.AscOrDescOrder(ESort.DESC, propertyName);
        int totalCount = list.Count();

        var start = (page - 1) * 10;
        list = list.Skip(start).Take(10);

        return new PagingResult<T>(list.ToList(), totalCount, true, $"{totalCount} record listed");
    }
    public Task<IQueryable<T>> GetAllQueryable()
    {
        IQueryable<T> queryable = Table;

        return Task.FromResult(queryable);
    }

    
}
