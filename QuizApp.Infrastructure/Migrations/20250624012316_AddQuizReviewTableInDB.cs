using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizReviewTableInDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsRecommended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizReviews", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizReviews_CreatedAt",
                table: "QuizReviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_QuizReviews_IsPublic",
                table: "QuizReviews",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_QuizReviews_QuizId",
                table: "QuizReviews",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizReviews_Rating",
                table: "QuizReviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_QuizReviews_UserId",
                table: "QuizReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizReviews_UserId_QuizId",
                table: "QuizReviews",
                columns: new[] { "UserId", "QuizId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizReviews");
        }
    }
}
