namespace OnionWebApi.Application.RedisCache;

public class RedisCacheSettings
{
    public string ConnectionString { get; set; }
    public string InstanceName { get; set; }
    public bool Enabled { get; set; }
}
