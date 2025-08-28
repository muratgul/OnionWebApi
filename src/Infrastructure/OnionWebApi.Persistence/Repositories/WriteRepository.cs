namespace OnionWebApi.Persistence.Repositories;
public class WriteRepository<T>(DbContext dbContext) : IWriteRepository<T> where T : class, IEntityBase, new()
{
    private readonly DbContext dbContext = dbContext;
    private DbSet<T> Table => dbContext.Set<T>();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Table.AddAsync(entity, cancellationToken);
    }
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await Table.AddRangeAsync(entities, cancellationToken);
    }
    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Table.Update(entity);
        return Task.FromResult(entity);
    }
    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        Table.UpdateRange(entities);
        return Task.CompletedTask;
    }
    public Task<bool> HardDeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Table.Remove(entity);
        return Task.FromResult(true);
    }
    public Task<int> HardDeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        Table.RemoveRange(entityList);
        return Task.FromResult(entityList.Count);
    }
    public Task<bool> SoftDeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Table.Update(entity);
        return Task.FromResult(true);
    }
    public Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        var softDeletableEntities = entityList.OfType<ISoftDeletable>().ToList();

        if (softDeletableEntities.Count == 0)
        {
            return Task.FromResult(0);
        }

        foreach (var entity in softDeletableEntities)
        {
            entity.IsDeleted = true;
        }

        Table.UpdateRange(softDeletableEntities.Cast<T>());

        return Task.FromResult(softDeletableEntities.Count);
    }
}
