namespace OnionWebApi.Persistence.Repositories;
public class CustomRepository<T>(DbContext dbContext) : ICustomRepository<T>
{
    private readonly DbContext dbContext = dbContext;
}
