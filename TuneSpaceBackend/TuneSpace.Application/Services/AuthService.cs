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

    async Task IAuthService.Register(string name, string email, string password, Roles role)
    {
        if (await _userRepository.GetUserByName(name) is not null)
        {
            throw new ArgumentException("Username already taken");
        }

        var user = new User
        {
            UserName = name,
            Email = email,
            Role = role
        };

        await _userRepository.InsertUser(user, password);
    }

    async Task<LoginResponse> IAuthService.Login(string email, string password)
    {
        var user = await _userRepository.GetUserByEmail(email);

        if (user is null || VerifyPassword(user, password) is false)
        {
            throw new UnauthorizedException("Incorrect email or password");
        }

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = await _tokenService.CreateRefreshToken(user);

        return new LoginResponse(user.Id.ToString(), user.UserName, user.Role, accessToken, refreshToken);
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
