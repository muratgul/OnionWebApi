namespace OnionWebApi.Api.Middlewares;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public IdempotencyMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        var idempotentAttribute = actionDescriptor?.MethodInfo.GetCustomAttribute<IdempotentAttribute>();

        if (idempotentAttribute is null)
        {
            await _next(context);
            return;
        }

        var key = context.Request.Headers["Idempotency-Key"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(key))
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            var errorMessage = "{\"error\": \"Idempotency-Key header is required.\"}";
            await context.Response.WriteAsync(errorMessage);
            return;
        }

        if (_cache.TryGetValue(key, out CachedResponse? cachedResponse))
        {
            context.Response.StatusCode = cachedResponse.StatusCode;
            context.Response.ContentType = cachedResponse.ContentType;
            context.Response.Headers.Add("X-Cache", "HIT");
            await context.Response.WriteAsync(cachedResponse.Body);
            return;
        }

        var originalBody = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        if (context.Response.StatusCode <= 200 && context.Response.StatusCode < 300)
        {
            var cached = new CachedResponse
            {
                StatusCode = context.Response.StatusCode,
                ContentType = context.Response.ContentType ?? "application/json",
                Body = responseBody
            };
            var cacheTime = TimeSpan.FromMinutes(idempotentAttribute.CacheMinutes);
            _cache.Set(key, cached, cacheTime);

            context.Response.Headers.Add("X-Cache", "MISS");
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);

        context.Response.Body = originalBody;
    }

    private class CachedResponse
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; } = "application/json";
        public string Body { get; set; } = string.Empty;
    }
}
