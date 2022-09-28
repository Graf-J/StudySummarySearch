using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySummarySearch.API.Migrations
{
    public partial class PropertyAddings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Semesters_Summaries_SummaryId",
                table: "Semesters");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Summaries_SummaryId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SummaryId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Semesters_SummaryId",
                table: "Semesters");

            migrationBuilder.DropColumn(
                name: "SummaryId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SummaryId",
                table: "Semesters");

            migrationBuilder.AlterColumn<string>(
                name: "URL",
                table: "Summaries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Summaries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Summaries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_SemesterId",
                table: "Summaries",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_SubjectId",
                table: "Summaries",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Summaries_Semesters_SemesterId",
                table: "Summaries",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Summaries_Subjects_SubjectId",
                table: "Summaries",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Summaries_Semesters_SemesterId",
                table: "Summaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Summaries_Subjects_SubjectId",
                table: "Summaries");

            migrationBuilder.DropIndex(
                name: "IX_Summaries_SemesterId",
                table: "Summaries");

            migrationBuilder.DropIndex(
                name: "IX_Summaries_SubjectId",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Summaries");

            migrationBuilder.AlterColumn<string>(
                name: "URL",
                table: "Summaries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SummaryId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SummaryId",
                table: "Semesters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SummaryId",
                table: "Subjects",
                column: "SummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_SummaryId",
                table: "Semesters",
                column: "SummaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Semesters_Summaries_SummaryId",
                table: "Semesters",
                column: "SummaryId",
                principalTable: "Summaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Summaries_SummaryId",
                table: "Subjects",
                column: "SummaryId",
                principalTable: "Summaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
