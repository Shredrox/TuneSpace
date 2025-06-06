using Microsoft.Extensions.Options;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Infrastructure.Options;

namespace TuneSpace.Infrastructure.Services;

internal class UrlBuilderService(IOptions<FrontendOptions> frontendOptions) : IUrlBuilderService
{
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;

    string IUrlBuilderService.BuildEmailConfirmationUrl(string userId, string token)
    {
        var baseUrl = _frontendOptions.BaseUrl.TrimEnd('/');
        return $"{baseUrl}/auth/confirm-email?userId={userId}&token={Uri.EscapeDataString(token)}";
    }

    string IUrlBuilderService.BuildPasswordResetUrl(string userId, string token)
    {
        var baseUrl = _frontendOptions.BaseUrl.TrimEnd('/');
        return $"{baseUrl}/auth/reset-password?userId={userId}&token={Uri.EscapeDataString(token)}";
    }

    string IUrlBuilderService.BuildEmailChangeConfirmationUrl(string userId, string token, string newEmail)
    {
        var baseUrl = _frontendOptions.BaseUrl.TrimEnd('/');
        return $"{baseUrl}/auth/confirm-email-change?userId={userId}&token={Uri.EscapeDataString(token)}&newEmail={Uri.EscapeDataString(newEmail)}";
    }
}
