using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class RecommendationContextRepository(TuneSpaceDbContext context) : IRecommendationContextRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<RecommendationContext?> IRecommendationContextRepository.GetByUserIdAsync(string userId)
    {
        return await _context.RecommendationContexts
            .FirstOrDefaultAsync(rc => rc.UserId == userId);
    }

    async Task<List<RecommendationContext>> IRecommendationContextRepository.GetSimilarUsersAsync(Vector userEmbedding, int limit)
    {
        return await _context.RecommendationContexts
            .Where(rc => rc.UserPreferenceEmbedding != null)
            .OrderBy(rc => rc.UserPreferenceEmbedding!.CosineDistance(userEmbedding))
            .Take(limit)
            .ToListAsync();
    }

    async Task<RecommendationContext> IRecommendationContextRepository.CreateOrUpdateAsync(RecommendationContext context)
    {
        var existing = await ((IRecommendationContextRepository)this).GetByUserIdAsync(context.UserId);

        if (existing is not null)
        {
            existing.UserGenres = context.UserGenres;
            existing.UserLocation = context.UserLocation;
            existing.UserTopArtists = context.UserTopArtists;
            existing.UserRecentlyPlayed = context.UserRecentlyPlayed;
            existing.UserPreferenceEmbedding = context.UserPreferenceEmbedding;
            existing.RetrievedContext = context.RetrievedContext;
            existing.LastUpdated = DateTime.UtcNow;

            _context.RecommendationContexts.Update(existing);
            await _context.SaveChangesAsync();

            return existing;
        }

        context.CreatedAt = DateTime.UtcNow;
        context.LastUpdated = DateTime.UtcNow;

        _context.RecommendationContexts.Add(context);
        await _context.SaveChangesAsync();

        return context;
    }

    async Task<bool> IRecommendationContextRepository.DeleteAsync(string userId)
    {
        var context = await ((IRecommendationContextRepository)this).GetByUserIdAsync(userId);
        if (context is null)
        {
            return false;
        }

        _context.RecommendationContexts.Remove(context);
        await _context.SaveChangesAsync();

        return true;
    }
}
