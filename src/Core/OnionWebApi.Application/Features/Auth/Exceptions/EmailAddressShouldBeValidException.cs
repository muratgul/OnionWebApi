namespace OnionWebApi.Application.Features.Auth.Exceptions;
public class EmailAddressShouldBeValidException : BaseException
{
    public EmailAddressShouldBeValidException() : base("Böyle bir email adresi bulunmamaktadır.") { }
}
