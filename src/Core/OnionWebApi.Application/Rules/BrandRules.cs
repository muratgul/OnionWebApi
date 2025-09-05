namespace OnionWebApi.Application.Rules;
public class BrandRules : BaseRules
{
    public Task BrandNameCheck(string brandName)
    {
        return brandName == "Foo" ? throw new Exception("Geçersiz isim") : Task.CompletedTask;
    }
}
