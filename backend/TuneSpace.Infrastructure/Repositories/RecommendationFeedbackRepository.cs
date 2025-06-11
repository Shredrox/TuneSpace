using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class RecommendationFeedbackRepository(TuneSpaceDbContext context) : IRecommendationFeedbackRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<RecommendationFeedback?> IRecommendationFeedbackRepository.GetByIdAsync(Guid id)
    {
        return await _context.RecommendationFeedbacks.FindAsync(id);
    }

    async Task<List<RecommendationFeedback>> IRecommendationFeedbackRepository.GetByUserIdAsync(string userId)
    {
        return await _context.RecommendationFeedbacks
            .Where(rf => rf.UserId == userId)
            .OrderByDescending(rf => rf.FeedbackAt)
            .ToListAsync();
    }

    async Task<List<RecommendationFeedback>> IRecommendationFeedbackRepository.GetUserFeedbackHistoryAsync(string userId, int daysPeriod)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysPeriod);
        return await _context.RecommendationFeedbacks
            .Where(rf => rf.UserId == userId && rf.FeedbackAt >= cutoffDate)
            .OrderByDescending(rf => rf.FeedbackAt)
            .ToListAsync();
    }

    public async Task<List<RecommendationFeedback>> GetByBandIdAsync(string bandId)
    {
        return await _context.RecommendationFeedbacks
            .Where(rf => rf.BandId == bandId)
            .OrderByDescending(rf => rf.FeedbackAt)
            .ToListAsync();
    }

    public async Task<List<RecommendationFeedback>> GetByUserAndBandAsync(string userId, string bandId)
    {
        return await _context.RecommendationFeedbacks
            .Where(rf => rf.UserId == userId && rf.BandId == bandId)
            .OrderByDescending(rf => rf.FeedbackAt)
            .ToListAsync();
    }

    async Task<int> IRecommendationFeedbackRepository.GetCountByUserAndTypeAsync(string userId, FeedbackType feedbackType)
    {
        return await _context.RecommendationFeedbacks
            .CountAsync(rf => rf.UserId == userId && rf.FeedbackType == feedbackType);
    }

    async Task<int> IRecommendationFeedbackRepository.GetTotalCountByUserAsync(string userId)
    {
        return await _context.RecommendationFeedbacks
            .CountAsync(rf => rf.UserId == userId);
    }

    async Task<RecommendationFeedback> IRecommendationFeedbackRepository.InsertAsync(RecommendationFeedback feedback)
    {
        _context.RecommendationFeedbacks.Add(feedback);
        await _context.SaveChangesAsync();
        return feedback;
    }

    async Task IRecommendationFeedbackRepository.UpdateAsync(RecommendationFeedback feedback)
    {
        _context.RecommendationFeedbacks.Update(feedback);
        await _context.SaveChangesAsync();
    }

    async Task IRecommendationFeedbackRepository.DeleteAsync(Guid id)
    {
        var feedback = await _context.RecommendationFeedbacks.FindAsync(id);
        if (feedback != null)
        {
            _context.RecommendationFeedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
        }
    }

    async Task<Dictionary<string, double>> IRecommendationFeedbackRepository.GetUserEngagementMetricsAsync(string userId)
    {
        var feedbacks = await _context.RecommendationFeedbacks
            .Where(rf => rf.UserId == userId)
            .ToListAsync();

        if (feedbacks.Count == 0)
        {
            return new Dictionary<string, double>
            {
                ["clickRate"] = 0.0,
                ["playRate"] = 0.0,
                ["followRate"] = 0.0,
                ["shareRate"] = 0.0,
                ["saveRate"] = 0.0,
                ["averageRating"] = 0.0,
                ["totalInteractions"] = 0.0
            };
        }

        var totalCount = feedbacks.Count;
        var clickCount = feedbacks.Count(f => f.Clicked);
        var playCount = feedbacks.Count(f => f.PlayedTrack); var followCount = feedbacks.Count(f => f.FollowedBand);
        var shareCount = feedbacks.Count(f => f.SharedRecommendation);
        var saveCount = feedbacks.Count(f => f.SavedForLater);
        var ratingsCount = feedbacks.Count(f => f.ExplicitRating > 0);
        var averageRating = ratingsCount > 0 ? feedbacks.Where(f => f.ExplicitRating > 0).Average(f => f.ExplicitRating) : 0.0;

        return new Dictionary<string, double>
        {
            ["clickRate"] = totalCount > 0 ? (double)clickCount / totalCount : 0.0,
            ["playRate"] = totalCount > 0 ? (double)playCount / totalCount : 0.0,
            ["followRate"] = totalCount > 0 ? (double)followCount / totalCount : 0.0,
            ["shareRate"] = totalCount > 0 ? (double)shareCount / totalCount : 0.0,
            ["saveRate"] = totalCount > 0 ? (double)saveCount / totalCount : 0.0,
            ["averageRating"] = averageRating,
            ["totalInteractions"] = totalCount
        };
    }

    async Task<double> IRecommendationFeedbackRepository.GetUserSuccessRateAsync(string userId)
    {
        var feedbacks = await _context.RecommendationFeedbacks
            .Where(rf => rf.UserId == userId)
            .ToListAsync();

        if (feedbacks.Count == 0)
        {
            return 0.0;
        }

        var positiveCount = feedbacks.Count(f => f.FeedbackType == FeedbackType.Positive);
        return (double)positiveCount / feedbacks.Count;
    }
}
