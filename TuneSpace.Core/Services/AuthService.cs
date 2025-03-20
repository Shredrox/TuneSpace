using Microsoft.AspNetCore.Identity;
using TuneSpace.Core.DTOs.Responses.Auth;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Enums;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Core.Services;

internal class AuthService(
    IUserRepository userRepository, 
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService) : IAuthService
{
    async Task IAuthService.Register(string name, string email, string password, UserRole role)
    {
        if (await userRepository.GetUserByName(name) is not null)
        {
            throw new ArgumentException("Username already taken");
        }
        
        var user = new User
        {
            UserName = name,
            Email = email,
            Role = role
        };

        await userRepository.InsertUser(user, password);
    }

    async Task<LoginResponse> IAuthService.Login(string email, string password)
    {
        var user = await userRepository.GetUserByEmail(email);

        if (user is null || VerifyPassword(user, password) is false)
        {
            throw new UnauthorizedException("Incorrect email or password");
        }

        var accessToken = tokenService.CreateAccessToken(user);
        var refreshToken = await tokenService.CreateRefreshToken(user);

        return new LoginResponse(user.Id, user.UserName, user.Role, accessToken, refreshToken);
    }
    
    private bool VerifyPassword(User user, string password)
    {
        if (user.PasswordHash is null)
        {
            return false;
        }
        
        return passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) is PasswordVerificationResult.Success;
    }
}