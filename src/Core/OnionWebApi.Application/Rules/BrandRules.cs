namespace OnionWebApi.Application.Rules;
public class BrandRules : BaseRules
{
    public Task BrandNameCheck(string brandName)
    {
        if (brandName == "Foo")
            throw new Exception("Geçersiz isim");

        return Task.CompletedTask;
    }
}
