namespace OnionWebApi.Application.Interfaces.Cache;
public interface IRedisCacheService
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, DateTime? expirationTime = null);
    Task RemoveAsync(string key);
    bool IsEnabled { get; }
}
