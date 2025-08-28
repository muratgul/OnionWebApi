namespace OnionWebApi.Application.Wrappers;
public class PaginationRequest<TEntity> where TEntity : class
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Expression<Func<TEntity, bool>>? Predicate { get; set; }
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; set; }
    public Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include { get; set; }
    public bool EnableTracking { get; set; } = false; 
    public string Route { get; set; } = string.Empty;
    public string? Fields { get; set; }
}
