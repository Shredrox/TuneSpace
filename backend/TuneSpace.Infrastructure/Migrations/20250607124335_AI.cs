using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuneSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicScoringWeights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    GenreMatchWeight = table.Column<double>(type: "double precision", nullable: false),
                    LocationMatchWeight = table.Column<double>(type: "double precision", nullable: false),
                    ListenerScoreWeight = table.Column<double>(type: "double precision", nullable: false),
                    SimilarArtistWeight = table.Column<double>(type: "double precision", nullable: false),
                    UndergroundBandWeight = table.Column<double>(type: "double precision", nullable: false),
                    NewReleaseWeight = table.Column<double>(type: "double precision", nullable: false),
                    RegisteredBandWeight = table.Column<double>(type: "double precision", nullable: false),
                    ExplorationFactor = table.Column<double>(type: "double precision", nullable: false),
                    DiversityFactor = table.Column<double>(type: "double precision", nullable: false),
                    LearningRate = table.Column<double>(type: "double precision", nullable: false),
                    RecommendationCount = table.Column<int>(type: "integer", nullable: false),
                    PositiveFeedbackCount = table.Column<int>(type: "integer", nullable: false),
                    SuccessRate = table.Column<double>(type: "double precision", nullable: false),
                    GenreMatchConfidence = table.Column<double>(type: "double precision", nullable: false),
                    LocationMatchConfidence = table.Column<double>(type: "double precision", nullable: false),
                    SimilarArtistConfidence = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAdaptation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicScoringWeights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenreEvolutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Genre = table.Column<string>(type: "text", nullable: false),
                    CurrentPreference = table.Column<double>(type: "double precision", nullable: false),
                    PreviousPreference = table.Column<double>(type: "double precision", nullable: false),
                    PreferenceChange = table.Column<double>(type: "double precision", nullable: false),
                    PreferenceVelocity = table.Column<double>(type: "double precision", nullable: false),
                    FirstEncountered = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EncounterCount = table.Column<int>(type: "integer", nullable: false),
                    MonthlyPreferences = table.Column<string>(type: "text", nullable: false),
                    WeeklyPreferences = table.Column<string>(type: "text", nullable: false),
                    LifecycleStage = table.Column<int>(type: "integer", nullable: false),
                    PredictionConfidence = table.Column<double>(type: "double precision", nullable: false),
                    NextUpdatePredicted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SocialInfluence = table.Column<double>(type: "double precision", nullable: false),
                    TrendInfluence = table.Column<double>(type: "double precision", nullable: false),
                    SeasonalInfluence = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreEvolutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecommendationFeedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    BandId = table.Column<string>(type: "text", nullable: false),
                    BandName = table.Column<string>(type: "text", nullable: false),
                    RecommendedGenres = table.Column<string>(type: "text", nullable: false),
                    InitialScore = table.Column<double>(type: "double precision", nullable: false),
                    RecommendedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FeedbackType = table.Column<int>(type: "integer", nullable: false),
                    FeedbackAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExplicitRating = table.Column<double>(type: "double precision", nullable: false),
                    Clicked = table.Column<bool>(type: "boolean", nullable: false),
                    PlayedTrack = table.Column<bool>(type: "boolean", nullable: false),
                    FollowedBand = table.Column<bool>(type: "boolean", nullable: false),
                    SharedRecommendation = table.Column<bool>(type: "boolean", nullable: false),
                    SavedForLater = table.Column<bool>(type: "boolean", nullable: false),
                    TimeSpentListening = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ScoringFactors = table.Column<string>(type: "text", nullable: false),
                    CalculatedSuccess = table.Column<double>(type: "double precision", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicScoringWeights_UserId",
                table: "DynamicScoringWeights",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenreEvolutions_UserId_Genre",
                table: "GenreEvolutions",
                columns: new[] { "UserId", "Genre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationFeedbacks_BandId",
                table: "RecommendationFeedbacks",
                column: "BandId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationFeedbacks_RecommendedAt",
                table: "RecommendationFeedbacks",
                column: "RecommendedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationFeedbacks_UserId",
                table: "RecommendationFeedbacks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicScoringWeights");

            migrationBuilder.DropTable(
                name: "GenreEvolutions");

            migrationBuilder.DropTable(
                name: "RecommendationFeedbacks");
        }
    }
}
