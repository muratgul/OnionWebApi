namespace OnionWebApi.Infrastructure.Cache;
public class RedisCacheSettings : IRedisCacheSettings
{
    public string ConnectionString { get; set; }
    public string InstanceName { get; set; }
    public bool Enabled { get; set; }
}
