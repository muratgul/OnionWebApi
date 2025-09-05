namespace OnionWebApi.Application.Features.Brands.Exceptions;
public class BrandNotFoundException : BaseException
{
    public BrandNotFoundException() : base("Brand not found.") { }
}
