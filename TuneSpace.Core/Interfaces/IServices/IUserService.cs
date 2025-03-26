using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

public interface IUserService
{
    Task<User?> GetUserByName(string name);
    Task<User?> GetUserFromRefreshToken(string refreshToken);
    Task<List<string>> SearchByName(string search);
    Task UpdateUserRefreshToken(User user);
}