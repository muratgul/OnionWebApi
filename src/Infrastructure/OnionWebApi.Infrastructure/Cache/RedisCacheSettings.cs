namespace OnionWebApi.Infrastructure.Cache;

public class RedisCacheSettings
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; }
    public string InstanceName { get; set; }
}