using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

public interface ITokenService
{
    string CreateAccessToken(User user);
    Task<string> CreateRefreshToken(User user);
}
