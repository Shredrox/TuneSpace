using Microsoft.AspNetCore.Identity;
using TuneSpace.Core.DTOs.Responses.Auth;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Enums;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class AuthService(
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

    private bool VerifyPassword(User user, string password)
    {
        if (user.PasswordHash is null)
        {
            return false;
        }

        return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) is PasswordVerificationResult.Success;
    }
}
