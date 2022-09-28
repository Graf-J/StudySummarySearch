using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySummarySearch.API.Migrations
{
    public partial class UserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DropboxAccessToken",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DropboxAccessToken", "PasswordHash", "PasswordSalt", "Role", "Username" },
                values: new object[] { 1, null, new byte[] { 38, 182, 45, 11, 166, 57, 149, 161, 45, 25, 223, 182, 150, 194, 17, 88, 25, 233, 17, 117, 203, 138, 203, 154, 252, 22, 142, 39, 108, 119, 39, 183, 244, 2, 119, 225, 193, 73, 116, 193, 99, 192, 215, 191, 242, 224, 176, 61, 84, 193, 13, 98, 63, 16, 50, 250, 243, 77, 205, 64, 58, 198, 255, 205 }, new byte[] { 135, 51, 50, 60, 57, 100, 68, 185, 91, 72, 100, 100, 82, 236, 22, 133, 253, 135, 74, 63, 143, 18, 98, 36, 253, 80, 18, 105, 113, 124, 134, 97, 25, 228, 214, 177, 193, 232, 104, 72, 81, 174, 41, 108, 145, 195, 80, 119, 231, 236, 60, 174, 21, 193, 244, 191, 24, 140, 231, 83, 175, 166, 47, 25, 226, 20, 11, 57, 151, 65, 151, 103, 51, 46, 75, 247, 223, 0, 186, 215, 172, 34, 25, 252, 93, 214, 147, 60, 47, 229, 94, 173, 89, 98, 88, 159, 146, 118, 99, 210, 32, 201, 62, 241, 224, 94, 220, 53, 130, 232, 2, 106, 52, 202, 225, 196, 55, 67, 78, 214, 194, 102, 13, 62, 227, 162, 70, 7 }, "SuperUser", "AdminGrafJ" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "DropboxAccessToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }
    }
}
