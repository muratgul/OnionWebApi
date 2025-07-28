namespace OnionWebApi.Application.Interfaces.UnitOfWorks;
public interface IUnitOfWork : IAsyncDisposable
{
    ICustomRepository<T> GetCustomRepository<T>() where T : class, IEntityBase, new();
    IReadRepository<T> GetReadRepository<T>() where T : class, IEntityBase, new();
    IWriteRepository<T> GetWriteRepository<T>() where T : class, IEntityBase, new();
    Task<int> SaveAsync();
    int Save();
}
