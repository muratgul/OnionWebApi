using System.Collections.Concurrent;

namespace OnionWebApi.Infrastructure.Cache;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ConcurrentDictionary<string, HashSet<string>> _tagToKeys;
    private readonly ConcurrentDictionary<string, HashSet<string>> _keyToTags;
    private readonly object _lock = new object();

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _tagToKeys = new ConcurrentDictionary<string, HashSet<string>>();
        _keyToTags = new ConcurrentDictionary<string, HashSet<string>>();
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_memoryCache.Get<T>(key));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }

        // Key silindiðinde tag mapping'lerini de temizle
        options.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration
        {
            EvictionCallback = (cacheKey, value, reason, state) =>
            {
                RemoveKeyFromTagMappings(cacheKey.ToString()!);
            }
        });

        _memoryCache.Set(key, value, options);

        // Tag mapping'lerini güncelle
        if (tags != null && tags.Any())
        {
            UpdateTagMappings(key, tags);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        RemoveKeyFromTagMappings(key);
        return Task.CompletedTask;
    }

    public Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        if (_tagToKeys.TryGetValue(tag, out var keys))
        {
            var keysCopy = new List<string>();
            lock (_lock)
            {
                keysCopy.AddRange(keys);
            }

            foreach (var key in keysCopy)
            {
                _memoryCache.Remove(key);
                RemoveKeyFromTagMappings(key);
            }
        }

        return Task.CompletedTask;
    }
    private void UpdateTagMappings(string key, IEnumerable<string> tags)
    {
        lock (_lock)
        {
            // Önce eski tag mapping'lerini temizle
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

            // Yeni tag mapping'lerini ekle
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
