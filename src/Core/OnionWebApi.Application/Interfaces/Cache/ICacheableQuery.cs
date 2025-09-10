namespace OnionWebApi.Application.Interfaces.RedisCache;
public interface ICacheableQuery
{
    string CacheKey { get; }
    string? CacheTag { get; }
    double CacheTime { get; }
}
