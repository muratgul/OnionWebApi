namespace OnionWebApi.Application.Features.Brands.Exceptions;

public class BrandAlreadyExistsException : BaseException
{
    public BrandAlreadyExistsException() : base("Brand with the same name already exists.") { }
}