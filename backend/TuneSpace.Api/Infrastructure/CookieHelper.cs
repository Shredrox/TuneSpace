namespace TuneSpace.Api.Infrastructure;

public static class CookieHelper
{
    private const string ACCESS_TOKEN_NAME = "AccessToken";
    private const string REFRESH_TOKEN_NAME = "RefreshToken";

    private static readonly CookieOptions BaseCookieOptions = new()
    {
        HttpOnly = true,
        Secure = true,
        IsEssential = true,
        SameSite = SameSiteMode.None
    };

    public static void SetAuthTokens(HttpResponse response, string accessToken, string refreshToken)
    {
        SetAccessToken(response, accessToken);
        SetRefreshToken(response, refreshToken);
    }

    public static void SetAccessToken(HttpResponse response, string accessToken)
    {
        var cookieOptions = CreateCookieOptions(TimeSpan.FromMinutes(15));
        response.Cookies.Append(ACCESS_TOKEN_NAME, accessToken, cookieOptions);
    }

    public static void SetRefreshToken(HttpResponse response, string refreshToken)
    {
        var cookieOptions = CreateCookieOptions(TimeSpan.FromDays(7));
        response.Cookies.Append(REFRESH_TOKEN_NAME, refreshToken, cookieOptions);
    }

    public static string? GetAccessToken(HttpRequest request)
    {
        return request.Cookies[ACCESS_TOKEN_NAME];
    }

    public static string? GetRefreshToken(HttpRequest request)
    {
        return request.Cookies[REFRESH_TOKEN_NAME];
    }

    public static void ClearAuthTokens(HttpResponse response)
    {
        response.Cookies.Delete(ACCESS_TOKEN_NAME);
        response.Cookies.Delete(REFRESH_TOKEN_NAME);
    }

    private static CookieOptions CreateCookieOptions(TimeSpan expiration)
    {
        return new CookieOptions
        {
            HttpOnly = BaseCookieOptions.HttpOnly,
            Secure = BaseCookieOptions.Secure,
            IsEssential = BaseCookieOptions.IsEssential,
            SameSite = BaseCookieOptions.SameSite,
            Expires = DateTime.UtcNow.Add(expiration)
        };
    }
}
