using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizTableInDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Instructions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TimeLimit = table.Column<int>(type: "integer", nullable: false),
                    MaxAttempts = table.Column<int>(type: "integer", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CreatedAt",
                table: "Quizzes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CreatedByUserId",
                table: "Quizzes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_Difficulty",
                table: "Quizzes",
                column: "Difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_IsActive",
                table: "Quizzes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_IsPublic",
                table: "Quizzes",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_Title",
                table: "Quizzes",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
