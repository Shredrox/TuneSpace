using System.ComponentModel.DataAnnotations;

namespace TuneSpace.Core.DTOs.Requests.MusicDiscovery;

public record RecommendationFeedbackRequest(
    [Required]
    string ArtistName,

    [Required]
    string InteractionType,

    double Rating = 0.0
);
