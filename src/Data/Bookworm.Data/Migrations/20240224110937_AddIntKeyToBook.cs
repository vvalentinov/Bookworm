#nullable disable

namespace Bookworm.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AddIntKeyToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Books_BookId",
                table: "Comments");
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteBooks_Books_BookId",
                table: "FavoriteBooks");
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Books_BookId",
                table: "Ratings");
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorsBooks_Books_BookId",
                table: "AuthorsBooks");

            // Drop existing primary key constraint
            migrationBuilder.DropPrimaryKey(name: "PK_Books", table: "Books");

            // Drop existing indexes
            migrationBuilder.DropIndex(
                name: "IX_Comments_BookId",
                table: "Comments");
            migrationBuilder.DropIndex(
                name: "IX_FavoriteBooks_BookId",
                table: "FavoriteBooks");
            migrationBuilder.DropIndex(
                name: "IX_Ratings_BookId",
                table: "Ratings");
            migrationBuilder.DropIndex(
                name: "IX_AuthorsBooks_BookId",
                table: "AuthorsBooks");

            // Rename existing Id column to OldId
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Books",
                newName: "OldId");

            // Add a new Id column with int type and set it as the primary key
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "AuthorsBooks",
                newName: "OldBookId");
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "AuthorsBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Ratings",
                newName: "OldBookId");
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Ratings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "FavoriteBooks",
                newName: "OldBookId");
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "FavoriteBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Comments",
                newName: "OldBookId");
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Update foreign keys to use the new Id column
            migrationBuilder.Sql("update [dbo].AuthorsBooks set BookId=t.Id FROM[dbo].AuthorsBooks i inner join dbo.Books t on i.OldBookId = t.OldId");
            migrationBuilder.Sql("update [dbo].Ratings set BookId=t.Id FROM[dbo].Ratings i inner join dbo.Books t on i.OldBookId = t.OldId");
            migrationBuilder.Sql("update [dbo].FavoriteBooks set BookId=t.Id FROM[dbo].FavoriteBooks i inner join dbo.Books t on i.OldBookId = t.OldId");
            migrationBuilder.Sql("update [dbo].Comments set BookId=t.Id FROM[dbo].Comments i inner join dbo.Books t on i.OldBookId = t.OldId");

            // Set the new Id column as the primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "Id");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_AuthorsBooks_Books_BookId",
                table: "AuthorsBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Books_BookId",
                table: "Ratings",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteBooks_Books_BookId",
                table: "FavoriteBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Books_BookId",
                table: "Comments",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_Comments_BookId",
                table: "Comments",
                column: "BookId");
            migrationBuilder.CreateIndex(
                name: "IX_FavoriteBooks_BookId",
                table: "FavoriteBooks",
                column: "BookId");
            migrationBuilder.CreateIndex(
                name: "IX_Ratings_BookId",
                table: "Ratings",
                column: "BookId");
            migrationBuilder.CreateIndex(
                name: "IX_AuthorsBooks_BookId",
                table: "AuthorsBooks",
                column: "BookId");

            // Drop the OldId columns
            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Books");
            migrationBuilder.DropColumn(
                name: "OldBookId",
                table: "AuthorsBooks");
            migrationBuilder.DropColumn(
                name: "OldBookId",
                table: "Ratings");
            migrationBuilder.DropColumn(
                name: "OldBookId",
                table: "FavoriteBooks");
            migrationBuilder.DropColumn(
               name: "OldBookId",
               table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
