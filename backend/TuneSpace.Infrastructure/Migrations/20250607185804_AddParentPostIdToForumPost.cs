using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuneSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParentPostIdToForumPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentPostId",
                table: "ForumPosts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_ParentPostId",
                table: "ForumPosts",
                column: "ParentPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumPosts_ForumPosts_ParentPostId",
                table: "ForumPosts",
                column: "ParentPostId",
                principalTable: "ForumPosts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumPosts_ForumPosts_ParentPostId",
                table: "ForumPosts");

            migrationBuilder.DropIndex(
                name: "IX_ForumPosts_ParentPostId",
                table: "ForumPosts");

            migrationBuilder.DropColumn(
                name: "ParentPostId",
                table: "ForumPosts");
        }
    }
}
