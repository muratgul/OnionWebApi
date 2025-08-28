namespace OnionWebApi.Application.Features.Auth.Rules;
public class AuthRules : BaseRules
{
    public Task UserShouldNotBeExist(AppUser? user)
    {
        if (user is not null)
            throw new UserAlreadyExistException();
        return Task.CompletedTask;
    }
    public Task EmailOrPasswordShouldNotBeInvalid(AppUser? user, bool checkPassword)
    {
        if (user is null || !checkPassword)
            throw new EmailOrPasswordShouldNotBeInvalidException();
        return Task.CompletedTask;
    }
    public Task RefreshTokenShouldNotBeExpired(DateTime? expiryDate)
    {
        if (expiryDate <= DateTime.Now)
            throw new RefreshTokenShouldNotBeExpiredException();
        return Task.CompletedTask;
    }

    public Task EmailAddressShouldBeValid(AppUser? user)
    {
        if (user is null)
            throw new EmailAddressShouldBeValidException();
        return Task.CompletedTask;
    }

    public Task TokensShouldNotBeEmpty(string? accessToken, string? refreshToken)
    {
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            throw new TokensShouldNotBeEmptyException();
        return Task.CompletedTask;
    }

    public Task RefreshTokenShouldBeValid(AppUser? user, string refreshTokenFromCookie)
    {
        if (user is null || user.RefreshToken != refreshTokenFromCookie || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new RefreshTokenShouldBeValidException();
        return Task.CompletedTask;
    }
}
