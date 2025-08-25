namespace OnionWebApi.Infrastructure.Cache;
public class HybridCacheService : IHybridCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IRedisCacheSettings _redisSettings;
    public HybridCacheService(IMemoryCache memoryCache, IRedisCacheService redisCacheService, IRedisCacheSettings redisSettings)
    {
        _memoryCache = memoryCache;
        _redisCacheService = redisCacheService;
        _redisSettings = redisSettings;
    }
    public async Task<T> GetAsync<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T data))
        {
            return data;
        }

        if (_redisSettings.Enabled)
        {
            data = await _redisCacheService.GetAsync<T>(key);

            if (data is not null)
            {
                _memoryCache.Set(key, data, TimeSpan.FromMinutes(1));
            }
        }

        return data;
    }
    public async Task SetAsync<T>(string key, T value, TimeSpan memoryCacheTime, DateTime redisCacheTime)
    {
        _memoryCache.Set(key, value, memoryCacheTime);

        if (_redisSettings.Enabled)
        {
            await _redisCacheService.SetAsync(key, value, redisCacheTime);
        }
    }
    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return _redisCacheService.RemoveAsync($"{key}*");
    }
    
}
