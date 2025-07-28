namespace OnionWebApi.Infrastructure.RedisCache;

public class RedisCacheService : IRedisCacheService, IDisposable
{
    private readonly ConnectionMultiplexer _redisConnection;
    private readonly IDatabase _database;
    private readonly RedisCacheSettings _settings;
    public RedisCacheService(IOptions<RedisCacheSettings> options)
    {
        _settings = options.Value;
        var opt = ConfigurationOptions.Parse(_settings.ConnectionString);
        _redisConnection = ConnectionMultiplexer.Connect(opt);
        _database = _redisConnection.GetDatabase();
    }
    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? JsonConvert.DeserializeObject<T>(value) : default;
    }

    public async Task SetAsync<T>(string key, T value, DateTime? expirationTime = null)
    {
        TimeSpan? expiry = null;
        if (expirationTime.HasValue)
        {
            expiry = expirationTime.Value - DateTime.UtcNow;
        }

        await _database.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry);
    }
    public void Dispose() => _redisConnection?.Dispose();
}
