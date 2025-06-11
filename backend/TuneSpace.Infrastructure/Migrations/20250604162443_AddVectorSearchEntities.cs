using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace TuneSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVectorSearchEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "ArtistEmbeddings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArtistName = table.Column<string>(type: "text", nullable: false),
                    SpotifyId = table.Column<string>(type: "text", nullable: true),
                    BandcampUrl = table.Column<string>(type: "text", nullable: true),
                    Genres = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    Embedding = table.Column<Vector>(type: "vector(384)", nullable: true),
                    Followers = table.Column<int>(type: "integer", nullable: true),
                    Popularity = table.Column<decimal>(type: "numeric", nullable: true),
                    LastActive = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    SimilarArtists = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataSource = table.Column<string>(type: "text", nullable: false),
                    SourceMetadata = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistEmbeddings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecommendationContexts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UserGenres = table.Column<string>(type: "text", nullable: false),
                    UserLocation = table.Column<string>(type: "text", nullable: true),
                    UserTopArtists = table.Column<string>(type: "text", nullable: false),
                    UserRecentlyPlayed = table.Column<string>(type: "text", nullable: false),
                    UserPreferenceEmbedding = table.Column<Vector>(type: "vector(384)", nullable: true),
                    RetrievedContext = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationContexts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistEmbeddings_ArtistName",
                table: "ArtistEmbeddings",
                column: "ArtistName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtistEmbeddings_DataSource",
                table: "ArtistEmbeddings",
                column: "DataSource");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistEmbeddings_SpotifyId",
                table: "ArtistEmbeddings",
                column: "SpotifyId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationContexts_UserId",
                table: "RecommendationContexts",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistEmbeddings");

            migrationBuilder.DropTable(
                name: "RecommendationContexts");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");
        }
    }
}
