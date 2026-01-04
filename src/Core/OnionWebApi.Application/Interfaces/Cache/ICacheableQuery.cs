namespace OnionWebApi.Application.Interfaces.RedisCache;
public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan CacheDuration { get; }
    IEnumerable<string>? CacheTags { get; }
}
