namespace OnionWebApi.Application.Beheviors;
public class HybridCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cacheService;

    public HybridCacheBehavior(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if(request is ICacheableQuery query)
        {
            var cacheKey = query.CacheKey;
            var cacheTime = query.CacheTime;

            var cachedData = _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken).Result;

            if(cachedData is not null)
            {
                return cachedData;
            }

            var response = await next(cancellationToken);

            if(response is not null)
            {
                
                await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(cacheTime), cancellationToken);
            }

            return response;
        }
        return await next(cancellationToken);
    }
}
