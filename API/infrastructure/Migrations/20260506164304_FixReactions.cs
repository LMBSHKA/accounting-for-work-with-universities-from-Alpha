using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixReactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectReactions_ProjectId",
                table: "ProjectReactions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProjectReactions");

            migrationBuilder.AddColumn<int>(
                name: "ReactionType",
                table: "ProjectReactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProjectCommentReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectCommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReactionType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectCommentReactions_ProjectComments_ProjectCommentId",
                        column: x => x.ProjectCommentId,
                        principalTable: "ProjectComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectCommentReactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectReactions_ProjectId_CreatedByUserId",
                table: "ProjectReactions",
                columns: new[] { "ProjectId", "CreatedByUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCommentReactions_ProjectCommentId_UserId",
                table: "ProjectCommentReactions",
                columns: new[] { "ProjectCommentId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCommentReactions_UserId",
                table: "ProjectCommentReactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectCommentReactions");

            migrationBuilder.DropIndex(
                name: "IX_ProjectReactions_ProjectId_CreatedByUserId",
                table: "ProjectReactions");

            migrationBuilder.DropColumn(
                name: "ReactionType",
                table: "ProjectReactions");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ProjectReactions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectReactions_ProjectId",
                table: "ProjectReactions",
                column: "ProjectId");
        }
    }
}
