namespace Bookworm.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class ChangedColumnNamesInQuote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MovieName",
                table: "Quotes",
                newName: "MovieTitle");

            migrationBuilder.RenameColumn(
                name: "BookName",
                table: "Quotes",
                newName: "BookTitle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MovieTitle",
                table: "Quotes",
                newName: "MovieName");

            migrationBuilder.RenameColumn(
                name: "BookTitle",
                table: "Quotes",
                newName: "BookName");
        }
    }
}
