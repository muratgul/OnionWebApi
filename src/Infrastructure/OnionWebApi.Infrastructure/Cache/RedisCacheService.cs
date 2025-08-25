using OnionWebApi.Application.Interfaces.Cache;

namespace OnionWebApi.Infrastructure.Cache;

public class RedisCacheService : IRedisCacheService, IDisposable
{
    private readonly ConnectionMultiplexer _redisConnection;
    private readonly IDatabase _database;
    private readonly IRedisCacheSettings _settings;

    public bool IsEnabled => true;
    public RedisCacheService(IRedisCacheSettings settings)
    {
        _settings = settings;
        var opt = ConfigurationOptions.Parse(_settings.ConnectionString);
        _redisConnection = ConnectionMultiplexer.Connect(opt);
        _database = _redisConnection.GetDatabase();
    }
    public async Task<T> GetAsync<T>(string key)
    {
        if (!_settings.Enabled)
            return default!;

        var value = await _database.StringGetAsync($"{_settings.InstanceName}_{key}");
        return value.HasValue ? JsonConvert.DeserializeObject<T>(value) : default;
    }

    public async Task SetAsync<T>(string key, T value, DateTime? expirationTime = null)
    {
        if (!_settings.Enabled)
            return;

        TimeSpan? expiry = null;
        if (expirationTime.HasValue)
        {
            expiry = expirationTime.Value - DateTime.UtcNow;
        }

        await _database.StringSetAsync($"{_settings.InstanceName}_{key}", JsonConvert.SerializeObject(value), expiry);
    }

    public async Task RemoveAsync(string key)
    {
        if(!_settings.Enabled) return;

        await _database.KeyDeleteAsync($"{_settings.InstanceName}_{key}");
    }
    public void Dispose() => _redisConnection?.Dispose();
}
