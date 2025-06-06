using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using TuneSpace.Core.DTOs.Responses.Auth;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Enums;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal partial class AuthService(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;

    async Task IAuthService.RegisterAsync(string name, string email, string password, Roles role)
    {
        if (await _userRepository.GetUserByNameAsync(name) is not null)
        {
            throw new ArgumentException("Username already taken");
        }

        var user = new User
        {
            UserName = name,
            Email = email,
            Role = role
        };

        await _userRepository.InsertUserAsync(user, password);
    }

    async Task<LoginResponse> IAuthService.LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user is null || VerifyPassword(user, password) is false)
        {
            throw new UnauthorizedException("Incorrect email or password");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.SaveRefreshTokenAsync(user, refreshToken);

        return new LoginResponse(user.Id.ToString(), user.UserName, user.Role.ToString(), accessToken, refreshToken);
    }

    async Task<LoginResponse> IAuthService.ExternalLoginAsync(string externalId, string email, string displayName, string provider, string? profilePictureUrl)
    {
        var user = await _userRepository.GetUserByExternalIdAsync(externalId, provider);

        if (user is null)
        {
            user = await _userRepository.GetUserByEmailAsync(email);

            if (user is not null)
            {
                user.SpotifyId = externalId;
                user.ExternalProvider = provider;
                user.ExternalLoginLinkedAt = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user);
            }
            else
            {
                var sanitizedUsername = SanitizeUsername(displayName);
                user = new User
                {
                    UserName = sanitizedUsername,
                    Email = email,
                    EmailConfirmed = true,
                    Role = Roles.Listener,
                    SpotifyId = externalId,
                    ExternalProvider = provider,
                    ExternalLoginLinkedAt = DateTime.UtcNow
                };

                if (!string.IsNullOrEmpty(profilePictureUrl))
                {
                    try
                    {
                        using var httpClient = new HttpClient();
                        var imageBytes = await httpClient.GetByteArrayAsync(profilePictureUrl);
                        user.ProfilePicture = imageBytes;
                    }
                    catch
                    {
                        user.ProfilePicture = null;
                    }
                }

                await _userRepository.InsertExternalUserAsync(user);
            }
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.SaveRefreshTokenAsync(user, refreshToken);

        return new LoginResponse(user.Id.ToString(), user.UserName, user.Role.ToString(), accessToken, refreshToken);
    }

    async Task<bool> IAuthService.ConnectExternalAccountAsync(string userId, string externalId, string email, string displayName, string provider, string? profilePictureUrl)
    {
        var existingUser = await _userRepository.GetUserByExternalIdAsync(externalId, provider);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("This Spotify account is already linked to another user.");
        }

        var user = await _userRepository.GetUserByIdAsync(userId) ?? throw new InvalidOperationException("User not found.");

        if (!string.IsNullOrEmpty(user.SpotifyId))
        {
            throw new InvalidOperationException("User already has a Spotify account linked.");
        }

        user.SpotifyId = externalId;
        user.ExternalProvider = provider;
        user.ExternalLoginLinkedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(profilePictureUrl) && user.ProfilePicture is null)
        {
            try
            {
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(profilePictureUrl);
                user.ProfilePicture = imageBytes;
            }
            catch
            {
            }
        }

        await _userRepository.UpdateUserAsync(user);
        return true;
    }

    private bool VerifyPassword(User user, string password)
    {
        if (user.PasswordHash is null)
        {
            return false;
        }

        return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) is PasswordVerificationResult.Success;
    }

    private static string SanitizeUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return string.Concat("user", Guid.NewGuid().ToString("N").AsSpan(0, 8));
        }

        var sanitized = SanitizeRegex().Replace(username, "_");

        return sanitized.Length > 32 ? sanitized[..32] : sanitized;
    }

    [GeneratedRegex(@"[^\w\d]")]
    private static partial Regex SanitizeRegex();
}
