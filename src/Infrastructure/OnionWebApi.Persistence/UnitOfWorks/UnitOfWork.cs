using MediatR;

namespace OnionWebApi.Persistence.UnitOfWorks;
public class UnitOfWork(AppDbContext dbContext, IMediator mediator) : IUnitOfWork
{
    private readonly IMediator mediator = mediator;
    private readonly AppDbContext dbContext = dbContext;

    public async ValueTask DisposeAsync() => await dbContext.DisposeAsync();
    public int Save() => dbContext.SaveChanges();
    public async Task<int> SaveAsync()
    {
        var domainEntities = dbContext.ChangeTracker.Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents is not null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();
        domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());
        var result = await dbContext.SaveChangesAsync();

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }

        return result;
    }
    ICustomRepository<T> IUnitOfWork.GetCustomRepository<T>() => new CustomRepository<T>(dbContext);
    IReadRepository<T> IUnitOfWork.GetReadRepository<T>() => new ReadRepository<T>(dbContext);
    IWriteRepository<T> IUnitOfWork.GetWriteRepository<T>() => new WriteRepository<T>(dbContext);
}
