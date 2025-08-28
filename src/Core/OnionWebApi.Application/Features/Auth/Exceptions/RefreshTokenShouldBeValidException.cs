namespace OnionWebApi.Application.Features.Auth.Exceptions;
public class RefreshTokenShouldBeValidException : BaseException
{
    public RefreshTokenShouldBeValidException() : base("Geçersiz refresh token!") { }
}