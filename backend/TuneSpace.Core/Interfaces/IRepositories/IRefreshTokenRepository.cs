using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken?> GetByUserIdAsync(Guid userId);
    Task InsertAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task DeleteAsync(Guid id);
    Task<int> DeleteExpiredAsync();
    Task<bool> ExistsAsync(string token);
    Task<bool> IsRevokedAsync(string token);
}
