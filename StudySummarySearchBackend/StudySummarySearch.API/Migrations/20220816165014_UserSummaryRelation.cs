using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySummarySearch.API.Migrations
{
    public partial class UserSummaryRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Summaries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_AuthorId",
                table: "Summaries",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Summaries_Users_AuthorId",
                table: "Summaries",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Summaries_Users_AuthorId",
                table: "Summaries");

            migrationBuilder.DropIndex(
                name: "IX_Summaries_AuthorId",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Summaries");
        }
    }
}
