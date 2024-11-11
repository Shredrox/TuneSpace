using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Core.Services;

public class TokenService(
    IConfiguration configuration,
    IUserRepository userRepository) : ITokenService
{
    public string CreateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration.GetSection("Jwt:Key").Value!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        
        var token = new JwtSecurityToken(
            claims: claims, 
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);
        
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
    
    public async Task<string> CreateRefreshToken(User user)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenValidity = DateTime.Now.AddHours(2).ToUniversalTime(); 
        
        await userRepository.UpdateUser(user);
        
        return refreshToken;
    }
}