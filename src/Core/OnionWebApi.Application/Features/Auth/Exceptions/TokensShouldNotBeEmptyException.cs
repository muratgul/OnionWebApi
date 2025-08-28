namespace OnionWebApi.Application.Features.Auth.Exceptions;
public class TokensShouldNotBeEmptyException : BaseException
{
    public TokensShouldNotBeEmptyException() : base("Token bilgileri boş olamaz!") { }
}