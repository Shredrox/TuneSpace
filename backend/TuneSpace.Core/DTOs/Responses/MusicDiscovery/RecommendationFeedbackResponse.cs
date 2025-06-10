namespace TuneSpace.Core.DTOs.Responses.MusicDiscovery;

public record RecommendationFeedbackResponse(
    string Message,
    bool Success,
    DateTime ProcessedAt
);

public record BatchRecommendationFeedbackResponse(
    string Message,
    bool Success,
    int ProcessedCount,
    int FailedCount,
    DateTime ProcessedAt
);
