using System.Net;
using Newtonsoft.Json;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Api.Middleware;

public class SpotifyTokenRefreshMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ISpotifyService spotifyService)
    {
        var originalBodyStream = context.Response.Body;

        try
        {
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                var refreshToken = context.Request.Cookies["SpotifyRefreshToken"];
                
                if (!string.IsNullOrEmpty(refreshToken) && 
                    context.Request.Path.Value?.Contains("/api/Spotify/", StringComparison.OrdinalIgnoreCase) == true)
                {
                    try
                    {
                        var newTokens = await spotifyService.RefreshAccessToken(refreshToken);
                        
                        context.Response.Cookies.Append("SpotifyAccessToken", newTokens.AccessToken, new CookieOptions
                        {
                            Expires = DateTime.Now.AddHours(1),
                            HttpOnly = true,
                            Secure = true,
                            IsEssential = true,
                            Domain = "localhost",
                            SameSite = SameSiteMode.None
                        });
                        
                        context.Response.StatusCode = 498;
                        context.Response.ContentType = "application/json";
                        
                        memoryStream.SetLength(0);
                        var writer = new StreamWriter(memoryStream);
                        await writer.WriteAsync(JsonConvert.SerializeObject(new 
                        { 
                            message = "Access token refreshed. Please retry your request." 
                        }));
                        await writer.FlushAsync();
                    }
                    catch (SpotifyApiException)
                    {
                        
                    }
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}

public static class SpotifyTokenRefreshMiddlewareExtensions
{
    public static IApplicationBuilder UseSpotifyTokenRefresh(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SpotifyTokenRefreshMiddleware>();
    }
}