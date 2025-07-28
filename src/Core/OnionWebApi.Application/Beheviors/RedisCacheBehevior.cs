using Microsoft.Extensions.Options;
using OnionWebApi.Application.RedisCache;

namespace OnionWebApi.Application.Beheviors;
public class RedisCacheBehevior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IRedisCacheService redisCacheService;
    private readonly RedisCacheSettings _settings;

    public RedisCacheBehevior(IRedisCacheService redisCacheService, IOptions<RedisCacheSettings> settings)
    {
        this.redisCacheService = redisCacheService;
        _settings = settings.Value;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_settings.Enabled && request is ICacheableQuery query)
        {
            var cacheKey = query.CacheKey;
            var cacheTime = query.CacheTime;

            var cachedData = await redisCacheService.GetAsync<TResponse>(cacheKey);
            if (cachedData is not null)
                return cachedData;

            var response = await next();
            if (response is not null)
                await redisCacheService.SetAsync(cacheKey, response, DateTime.Now.AddMinutes(cacheTime));

            return response;
        }

        return await next();
    }
}
