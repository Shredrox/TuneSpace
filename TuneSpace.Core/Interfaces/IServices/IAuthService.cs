using TuneSpace.Core.DTOs.Responses.Auth;
using TuneSpace.Core.Enums;

namespace TuneSpace.Core.Interfaces.IServices;

public interface IAuthService
{
    Task Register(string name, string email, string password, UserRole role);
    Task<LoginResponse> Login(string email, string password);
}
