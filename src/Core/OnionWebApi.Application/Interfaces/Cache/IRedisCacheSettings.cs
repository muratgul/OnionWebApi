namespace OnionWebApi.Application.Interfaces.RedisCache;

public interface IRedisCacheSettings
{
    string ConnectionString { get; set; }
    string InstanceName { get; set; }
    bool Enabled { get; set; }
}
