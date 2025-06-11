using System.ComponentModel.DataAnnotations;

namespace TuneSpace.Core.DTOs.Requests.MusicDiscovery;

public record BatchRecommendationFeedbackRequest(
    [Required]
    List<BatchFeedbackItem> Interactions
);

public record BatchFeedbackItem(
    [Required]
    string ArtistName,

    [Required]
    string InteractionType,

    [Required]
    List<string> Genres,

    double Rating = 0.0
);
