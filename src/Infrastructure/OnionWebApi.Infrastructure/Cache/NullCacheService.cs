namespace OnionWebApi.Infrastructure.Cache;

public class NullCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null,
        IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
