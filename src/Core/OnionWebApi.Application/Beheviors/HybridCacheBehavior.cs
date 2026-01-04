using Microsoft.Extensions.Logging;

namespace OnionWebApi.Application.Beheviors;

public class HybridCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<HybridCacheBehavior<TRequest, TResponse>> _logger;
    private readonly bool _isCacheEnabled;

    public HybridCacheBehavior(ICacheService cacheService, IConfiguration configuration, ILogger<HybridCacheBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
        _isCacheEnabled = configuration.GetValue<bool>("CacheSettings:Enabled");
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_isCacheEnabled || request is not ICacheableQuery query)
        {
            return await next(cancellationToken);
        }

        var cacheKey = query.CacheKey;

        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            _logger.LogWarning("Cache key boş: {RequestType}", typeof(TRequest).Name);
            return await next(cancellationToken);
        }

        try
        {
            var cachedData = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);

            if (cachedData is not null)
            {
                _logger.LogDebug("Cache hit: {CacheKey}", cacheKey);
                return cachedData;
            }

            _logger.LogDebug("Cache miss: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache okuma hatası: {CacheKey}, devam ediliyor", cacheKey);
        }

        var response = await next(cancellationToken);

        if (response is not null)
        {
            try
            {
                await _cacheService.SetAsync(cacheKey, response, query.CacheDuration, query.CacheTags, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache yazma hatası: {CacheKey}", cacheKey);
            }
        }

        return response;
    }
    
}
