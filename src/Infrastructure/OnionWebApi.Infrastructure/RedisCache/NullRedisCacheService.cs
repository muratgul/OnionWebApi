namespace OnionWebApi.Infrastructure.RedisCache;
public class NullRedisCacheService : IRedisCacheService
{
    public bool IsEnabled => false;
    public Task<T> GetAsync<T>(string key) => default!;
    public Task RemoveAsync(string key) => Task.CompletedTask;
    public Task SetAsync<T>(string key, T value, DateTime? expirationTime = null) => Task.CompletedTask;
}
