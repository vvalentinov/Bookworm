#nullable disable

namespace Bookworm.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class UpdateQuotesLikesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuotesLikes",
                table: "QuotesLikes");

            migrationBuilder.DropIndex(
                name: "IX_QuotesLikes_IsDeleted",
                table: "QuotesLikes");

            migrationBuilder.DropIndex(
                name: "IX_QuotesLikes_QuoteId",
                table: "QuotesLikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "QuotesLikes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "QuotesLikes");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "QuotesLikes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "QuotesLikes");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "QuotesLikes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuotesLikes",
                table: "QuotesLikes",
                columns: new[] { "QuoteId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuotesLikes",
                table: "QuotesLikes");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "QuotesLikes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "QuotesLikes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "QuotesLikes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "QuotesLikes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "QuotesLikes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuotesLikes",
                table: "QuotesLikes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuotesLikes_IsDeleted",
                table: "QuotesLikes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuotesLikes_QuoteId",
                table: "QuotesLikes",
                column: "QuoteId");
        }
    }
}
