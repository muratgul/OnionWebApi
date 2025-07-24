namespace UnionWebApi.Persistence.Repositories;
public class CustomRepository<T> : ICustomRepository<T>
{
    private readonly DbContext dbContext;

    public CustomRepository(DbContext dbContext)
    {
        this.dbContext = dbContext;
    }
}
