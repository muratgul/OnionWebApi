namespace OnionWebApi.Application.Interfaces.Repositories;
public interface IWriteRepository<T> where T : class, IEntityBase, new()
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<bool> HardDeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> HardDeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(T entity, CancellationToken cancellationToken = default);
}
