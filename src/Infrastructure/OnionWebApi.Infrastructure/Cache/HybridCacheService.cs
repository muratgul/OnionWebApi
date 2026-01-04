namespace OnionWebApi.Infrastructure.Cache;
public class HybridCacheService : ICacheService
{
    private readonly HybridCache _hybridCache;
    public HybridCacheService(HybridCache hybridCache)
    {
        _hybridCache = hybridCache;
    }

    private static readonly HybridCacheEntryOptions ShortTermOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(2),
        Flags = HybridCacheEntryFlags.DisableCompression
    };

    private static readonly HybridCacheEntryOptions MediumTermOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(30),
        LocalCacheExpiration = TimeSpan.FromMinutes(10),
        Flags = HybridCacheEntryFlags.None
    };

    private static readonly HybridCacheEntryOptions LongTermOptions = new()
    {
        Expiration = TimeSpan.FromHours(2),
        LocalCacheExpiration = TimeSpan.FromMinutes(15),
        Flags = HybridCacheEntryFlags.None
    }; 

    private static HybridCacheEntryOptions? GetCacheOptions(TimeSpan? expiration)
    {
        if (!expiration.HasValue)
        {
            return MediumTermOptions;
        }

        // Pre-configured options kullan, performans için
        return expiration.Value switch
        {
            var exp when exp <= TimeSpan.FromMinutes(5) => ShortTermOptions,
            var exp when exp <= TimeSpan.FromMinutes(30) => MediumTermOptions,
            var exp when exp <= TimeSpan.FromHours(2) => LongTermOptions,
            _ => new HybridCacheEntryOptions
            {
                Expiration = expiration.Value,
                LocalCacheExpiration = TimeSpan.FromMinutes(Math.Min(15, expiration.Value.TotalMinutes * 0.5)),
                Flags = HybridCacheEntryFlags.None
            }
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var result = await _hybridCache.GetOrCreateAsync<T>(key,
                    factory: async (cancellationToken) => default(T)!,
                    cancellationToken: cancellationToken);

        return result;
    }
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        var options = GetCacheOptions(expiration);
        await _hybridCache.SetAsync(key, value, options, tags, cancellationToken);
    }
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _hybridCache.RemoveAsync(key, cancellationToken);
    }
    public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        await _hybridCache.RemoveByTagAsync(tag, cancellationToken);
    }

}