using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blanca_eManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class CategoriesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryModelId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryModelId",
                table: "Products",
                column: "CategoryModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryModelId",
                table: "Products",
                column: "CategoryModelId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryModelId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryModelId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryModelId",
                table: "Products");
        }
    }
}
