namespace OnionWebApi.Application.Interfaces.RedisCache;

public interface IRedisCacheSettings
{
    public string ConnectionString { get; set; }
    public string InstanceName { get; set; }
    public bool Enabled { get; set; }
}
