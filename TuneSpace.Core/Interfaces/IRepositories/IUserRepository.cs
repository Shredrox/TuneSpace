using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

public interface IUserRepository
{
    Task<User?> GetUserById(string id);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByName(string name);
    Task<User?> GetUserByRefreshToken(string refreshToken);
    Task<List<string>> SearchByName(string name);
    Task InsertUser(User user, string password);
    Task UpdateUser(User user);
}