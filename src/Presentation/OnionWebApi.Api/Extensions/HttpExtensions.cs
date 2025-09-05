namespace OnionWebApi.Api.Extensions;

public static class HttpExtensions
{
    public static void SetRefreshTokenCookie(this HttpResponse response, string refreshToken, DateTime expiryTime)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiryTime
        };
        response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    public static void DeleteRefreshTokenCookie(this HttpResponse response)
    {
        response.Cookies.Delete("refreshToken");
    }
}