#nullable disable

namespace Bookworm.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class UpdateAuthorBookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorsBooks_Books_BookId",
                table: "AuthorsBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorsBooks",
                table: "AuthorsBooks");

            migrationBuilder.DropIndex(
                name: "IX_AuthorsBooks_AuthorId",
                table: "AuthorsBooks");

            migrationBuilder.DropIndex(
                name: "IX_AuthorsBooks_IsDeleted",
                table: "AuthorsBooks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AuthorsBooks");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AuthorsBooks");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "AuthorsBooks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AuthorsBooks");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "AuthorsBooks");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "AuthorsBooks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorsBooks",
                table: "AuthorsBooks",
                columns: new[] { "AuthorId", "BookId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorsBooks_Books_BookId",
                table: "AuthorsBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorsBooks_Books_BookId",
                table: "AuthorsBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorsBooks",
                table: "AuthorsBooks");

            migrationBuilder.AlterColumn<string>(
                name: "BookId",
                table: "AuthorsBooks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AuthorsBooks",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "AuthorsBooks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "AuthorsBooks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AuthorsBooks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "AuthorsBooks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorsBooks",
                table: "AuthorsBooks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorsBooks_AuthorId",
                table: "AuthorsBooks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorsBooks_IsDeleted",
                table: "AuthorsBooks",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorsBooks_Books_BookId",
                table: "AuthorsBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id");
        }
    }
}
