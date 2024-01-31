﻿#nullable disable

namespace Bookworm.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AddLikesColumnToQuote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Quotes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
