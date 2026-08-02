using ZiggyCreatures.Caching.Fusion;
using OnionWebApi.Application.Interfaces.Cache;
using System.Collections.Concurrent;

namespace OnionWebApi.Infrastructure.Cache;

public class FusionCacheService : ICacheService
{
    private readonly IFusionCache _fusionCache;
    private readonly ConcurrentDictionary<string, HashSet<string>> _tagToKeys;
    private readonly ConcurrentDictionary<string, HashSet<string>> _keyToTags;
    private readonly object _lock = new object();

    public FusionCacheService(IFusionCache fusionCache)
    {
        _fusionCache = fusionCache;
        _tagToKeys = new ConcurrentDictionary<string, HashSet<string>>();
        _keyToTags = new ConcurrentDictionary<string, HashSet<string>>();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return await _fusionCache.GetOrDefaultAsync<T>(key, token: cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        var options = new FusionCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.SetDuration(expiration.Value);
        }
        else 
        {
            options.SetDuration(TimeSpan.FromMinutes(30)); // Default
        }

        await _fusionCache.SetAsync(key, value, options, token: cancellationToken);

        if (tags != null && tags.Any())
        {
            UpdateTagMappings(key, tags);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _fusionCache.RemoveAsync(key, token: cancellationToken);
        RemoveKeyFromTagMappings(key);
    }

    public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        if (_tagToKeys.TryRemove(tag, out var keys))
        {
            List<string> keysToRemove;
            lock (_lock)
            {
                keysToRemove = keys.ToList();
            }

            foreach (var key in keysToRemove)
            {
                await _fusionCache.RemoveAsync(key, token: cancellationToken);
                RemoveKeyFromTagMappings(key);
            }
        }
    }

    private void UpdateTagMappings(string key, IEnumerable<string> tags)
    {
        lock (_lock)
        {
            if (_keyToTags.TryGetValue(key, out var oldTags))
            {
                foreach (var oldTag in oldTags)
                {
                    if (_tagToKeys.TryGetValue(oldTag, out var keysSet))
                    {
                        keysSet.Remove(key);
                        if (keysSet.Count == 0)
                        {
                            _tagToKeys.TryRemove(oldTag, out _);
                        }
                    }
                }
            }

            var newTags = new HashSet<string>(tags);
            _keyToTags.AddOrUpdate(key, newTags, (k, v) => newTags);

            foreach (var tag in tags)
            {
                _tagToKeys.AddOrUpdate(tag,
                    new HashSet<string> { key },
                    (t, existingKeys) =>
                    {
                        existingKeys.Add(key);
                        return existingKeys;
                    });
            }
        }
    }

    private void RemoveKeyFromTagMappings(string key)
    {
        lock (_lock)
        {
            if (_keyToTags.TryRemove(key, out var tags))
            {
                foreach (var tag in tags)
                {
                    if (_tagToKeys.TryGetValue(tag, out var keysSet))
                    {
                        keysSet.Remove(key);
                        if (keysSet.Count == 0)
                        {
                            _tagToKeys.TryRemove(tag, out _);
                        }
                    }
                }
            }
        }
    }
}
