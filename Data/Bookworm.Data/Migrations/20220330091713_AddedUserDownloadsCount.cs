using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookworm.Data.Migrations
{
    public partial class AddedUserDownloadsCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownloadsCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadsCount",
                table: "AspNetUsers");
        }
    }
}
