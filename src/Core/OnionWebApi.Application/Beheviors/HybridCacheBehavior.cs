using Microsoft.Extensions.Configuration;

namespace OnionWebApi.Application.Beheviors;
public class HybridCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cacheService;
    private readonly IConfiguration _configuration;

    public HybridCacheBehavior(ICacheService cacheService, IConfiguration configuration)
    {
        _cacheService = cacheService;
        _configuration = configuration;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var isCacheEnabled = _configuration.GetValue<bool>("CacheSettings:Enabled");
        if (!isCacheEnabled)
        {
            return await next(cancellationToken);
        }
        if (request is ICacheableQuery query)
        {
            var cacheKey = query.CacheKey;
            var cacheTime = query.CacheTime;
            var cacheTag = query.CacheTag ?? "";

            var tags = new List<string>
            {
                cacheTag
            };

            var cachedData = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);

            if (cachedData is not null)
            {
                return cachedData;
            }

            var response = await next(cancellationToken);

            if (response is not null)
            {
                await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(cacheTime), tags, cancellationToken);
            }

            return response;
        }
        return await next(cancellationToken);
    }
}
