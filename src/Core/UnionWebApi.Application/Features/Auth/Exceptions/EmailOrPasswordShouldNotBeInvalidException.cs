namespace UnionWebApi.Application.Features.Auth.Exceptions;
public class EmailOrPasswordShouldNotBeInvalidException : BaseException
{
    public EmailOrPasswordShouldNotBeInvalidException() : base("Kullanıcı adı veya şifre yanlıştır.") { }

}
