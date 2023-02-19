using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySummarySearch.API.Migrations
{
    public partial class RemoveImageURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URL",
                table: "Summaries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "Summaries",
                type: "text",
                nullable: true);
        }
    }
}
