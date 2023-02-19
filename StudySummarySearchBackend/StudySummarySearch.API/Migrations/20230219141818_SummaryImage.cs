using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySummarySearch.API.Migrations
{
    public partial class SummaryImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DropboxAccessToken",
                table: "Users");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Summaries",
                type: "bytea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Summaries");

            migrationBuilder.AddColumn<string>(
                name: "DropboxAccessToken",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}
