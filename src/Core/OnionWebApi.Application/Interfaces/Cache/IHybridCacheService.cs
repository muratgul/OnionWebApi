namespace OnionWebApi.Application.Interfaces.Cache;
public interface IHybridCacheService
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan memoryCacheTime, DateTime redisCacheTime);
    Task RemoveAsync(string key);
}
