using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuneSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpotifyIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExternalLoginLinkedAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalProvider",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpotifyId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalLoginLinkedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ExternalProvider",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SpotifyId",
                table: "AspNetUsers");
        }
    }
}
