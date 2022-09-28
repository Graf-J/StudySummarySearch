using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySummarySearch.API.Migrations
{
    public partial class AnotherRelationsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Keywords_Summaries_SummaryId",
                table: "Keywords");

            migrationBuilder.DropIndex(
                name: "IX_Keywords_SummaryId",
                table: "Keywords");

            migrationBuilder.DropColumn(
                name: "SummaryId",
                table: "Keywords");

            migrationBuilder.CreateTable(
                name: "KeywordSummary",
                columns: table => new
                {
                    KeywordsId = table.Column<int>(type: "integer", nullable: false),
                    SummariesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordSummary", x => new { x.KeywordsId, x.SummariesId });
                    table.ForeignKey(
                        name: "FK_KeywordSummary_Keywords_KeywordsId",
                        column: x => x.KeywordsId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeywordSummary_Summaries_SummariesId",
                        column: x => x.SummariesId,
                        principalTable: "Summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeywordSummary_SummariesId",
                table: "KeywordSummary",
                column: "SummariesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeywordSummary");

            migrationBuilder.AddColumn<int>(
                name: "SummaryId",
                table: "Keywords",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_SummaryId",
                table: "Keywords",
                column: "SummaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Keywords_Summaries_SummaryId",
                table: "Keywords",
                column: "SummaryId",
                principalTable: "Summaries",
                principalColumn: "Id");
        }
    }
}
