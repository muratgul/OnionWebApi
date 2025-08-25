namespace OnionWebApi.Application.Beheviors;
public class RedisCacheBehevior<TRequest, TResponse>(IHybridCacheService hybridCacheService) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IHybridCacheService _cacheService = hybridCacheService;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is ICacheableQuery query)
        {
            var cacheKey = query.CacheKey;
            var cacheTime = query.CacheTime;

            var cachedData = await _cacheService.GetAsync<TResponse>(cacheKey);

            if (cachedData is not null)
            {
                return cachedData;
            }

            var response = await next(cancellationToken);

            if (response is not null)
            {
                await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(cacheTime), DateTime.Now.AddMinutes(cacheTime));
            }

            return response;
        }

        return await next(cancellationToken);
    }
}