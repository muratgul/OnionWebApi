namespace OnionWebApi.Persistence.UnitOfWorks;
public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    private readonly AppDbContext dbContext = dbContext;

    public async ValueTask DisposeAsync() => await dbContext.DisposeAsync();
    public int Save() => dbContext.SaveChanges();
    public async Task<int> SaveAsync() => await dbContext.SaveChangesAsync();
    ICustomRepository<T> IUnitOfWork.GetCustomRepository<T>() => new CustomRepository<T>(dbContext);
    IReadRepository<T> IUnitOfWork.GetReadRepository<T>() => new ReadRepository<T>(dbContext);
    IWriteRepository<T> IUnitOfWork.GetWriteRepository<T>() => new WriteRepository<T>(dbContext);
}
