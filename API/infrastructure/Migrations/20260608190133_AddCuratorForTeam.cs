using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCuratorForTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CuratorId",
                table: "Teams",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CuratorId",
                table: "Teams",
                column: "CuratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Users_CuratorId",
                table: "Teams",
                column: "CuratorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Users_CuratorId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_CuratorId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CuratorId",
                table: "Teams");
        }
    }
}
