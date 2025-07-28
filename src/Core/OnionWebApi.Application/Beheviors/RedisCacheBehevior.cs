namespace OnionWebApi.Application.Beheviors;
public class RedisCacheBehevior<TRequest, TResponse>(IRedisCacheService redisCacheService, IOptions<RedisCacheSettings> settings) : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IRedisCacheService _redisCacheService = redisCacheService;
    private readonly RedisCacheSettings _settings = settings.Value;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_settings.Enabled && request is ICacheableQuery query)
        {
            var cacheKey = query.CacheKey;
            var cacheTime = query.CacheTime;

            var cachedData = await _redisCacheService.GetAsync<TResponse>(cacheKey);
            if (cachedData is not null)
                return cachedData;

            var response = await next(cancellationToken);

            if (response is not null)
                await _redisCacheService.SetAsync(cacheKey, response, DateTime.Now.AddMinutes(cacheTime));

            return response;
        }

        return await next(cancellationToken);
    }
}
