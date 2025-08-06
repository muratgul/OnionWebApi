namespace OnionWebApi.Api.Attributes;

public class IdempotentAttribute : Attribute
{
    public int CacheMinutes { get; set; } = 20;
    public IdempotentAttribute() { }
    public IdempotentAttribute(int cacheMinutes)
    {
        CacheMinutes = cacheMinutes;
    }
}
