using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Organizer.Migrations
{
    public partial class ChangeRecordsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Records");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Records",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Records");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Records",
                nullable: true);
        }
    }
}
