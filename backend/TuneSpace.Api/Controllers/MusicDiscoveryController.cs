using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneSpace.Api.Extensions;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Core.DTOs.Requests.MusicDiscovery;
using TuneSpace.Core.DTOs.Responses.MusicDiscovery;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MusicDiscoveryController(
    IMusicDiscoveryService musicDiscoveryService,
    ILogger<MusicDiscoveryController> logger) : ControllerBase
{
    private readonly IMusicDiscoveryService _musicDiscoveryService = musicDiscoveryService;
    private readonly ILogger<MusicDiscoveryController> _logger = logger;

    [HttpGet("recommendations")]
    public async Task<IActionResult> GetRecommendations([FromQuery] string? genres, [FromQuery] string? location = null)
    {
        try
        {
            var accessToken = Request.Cookies["SpotifyAccessToken"];

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token is required");
            }

            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID is required");
            }

            var genresList = string.IsNullOrEmpty(genres)
                ? []
                : genres.Split(',').ToList();

            var recommendations = await _musicDiscoveryService.GetBandRecommendationsAsync(
                accessToken,
                userId.ToString(),
                genresList,
                location ?? "");

            return Ok(recommendations);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching band recommendations");
            return BadRequest(e.Message);
        }
    }

    [HttpPost("feedback")]
    public async Task<IActionResult> TrackRecommendationFeedback([FromBody] RecommendationFeedbackRequest request, [FromQuery] string? genres = null)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID is required");
            }

            var genresList = string.IsNullOrEmpty(genres)
                ? []
                : genres.Split(',').ToList();

            await _musicDiscoveryService.TrackRecommendationInteractionAsync(
                userId.ToString(),
                request.ArtistName,
                request.InteractionType,
                genresList,
                request.Rating);

            var response = new RecommendationFeedbackResponse(
                $"Successfully tracked {request.InteractionType} feedback for {request.ArtistName}",
                true,
                DateTime.UtcNow);

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error tracking recommendation feedback for artist {ArtistName}", request.ArtistName);

            var errorResponse = new RecommendationFeedbackResponse(
                "Failed to track recommendation feedback",
                false,
                DateTime.UtcNow);

            return BadRequest(errorResponse);
        }
    }

    [HttpPost("feedback/batch")]
    public async Task<IActionResult> TrackBatchRecommendationFeedback([FromBody] BatchRecommendationFeedbackRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID is required");
            }

            var interactions = request.Interactions
                .Select(i => (i.ArtistName, i.InteractionType, i.Rating, i.Genres))
                .ToList();

            await _musicDiscoveryService.TrackBatchRecommendationInteractionsAsync(
                userId.ToString(),
                interactions);

            var response = new BatchRecommendationFeedbackResponse(
                $"Successfully processed {request.Interactions.Count} feedback interactions",
                true,
                request.Interactions.Count,
                0,
                DateTime.UtcNow);

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error tracking batch recommendation feedback");

            var errorResponse = new BatchRecommendationFeedbackResponse(
                "Failed to track recommendation feedback",
                false,
                0,
                request.Interactions.Count,
                DateTime.UtcNow);

            return BadRequest(errorResponse);
        }
    }

    [HttpGet("recommendations/enhanced")]
    public async Task<IActionResult> GetEnhancedRecommendations([FromQuery] string? genres, [FromQuery] string? location = null)
    {
        try
        {
            var accessToken = Request.Cookies["SpotifyAccessToken"];

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token is required");
            }

            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID is required");
            }

            var genresList = string.IsNullOrEmpty(genres)
                ? []
                : genres.Split(',').ToList();

            var recommendations = await _musicDiscoveryService.GetEnhancedBandRecommendationsAsync(
                accessToken,
                userId.ToString(),
                genresList,
                location ?? "");

            var enhancedRecommendations = recommendations
                .Where(r => r.DataSource?.Contains("Enhanced AI") == true ||
                           r.DataSource?.Contains("Collaborative Filtering") == true)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    r.Genres,
                    r.Location,
                    r.RelevanceScore,
                    r.DataSource,
                    r.ExternalUrls,
                    r.SimilarToArtistName,
                    r.ExternalUrl,
                    ConfidenceScore = r.RelevanceScore,
                    IsEnhancedAI = r.DataSource?.Contains("Enhanced AI") == true,
                    IsCollaborativeFiltering = r.DataSource?.Contains("Collaborative Filtering") == true,
                    AdaptiveLearningEnabled = true
                })
                .OrderByDescending(r => r.ConfidenceScore)
                .ToList();

            return Ok(new
            {
                Message = "Enhanced AI recommendations with confidence scoring",
                TotalRecommendations = enhancedRecommendations.Count,
                HasAdaptiveLearning = true,
                ProcessedAt = DateTime.UtcNow,
                Recommendations = enhancedRecommendations
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching enhanced AI recommendations");
            return BadRequest(e.Message);
        }
    }
}
