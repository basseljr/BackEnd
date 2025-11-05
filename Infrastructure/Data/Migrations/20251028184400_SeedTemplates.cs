using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Templates",
                columns: new[] { "Id", "Category", "DemoUrl", "Description", "Name", "PreviewImage" },
                values: new object[,]
                {
                    { 1, "E-commerce", "https://demo.yoursite.com/perfume", "Elegant template for perfume shops.", "Perfume Store", "/assets/templates/perfume.jpg" },
                    { 2, "Food & Drink", "https://demo.yoursite.com/restaurant", "Modern single-page layout for restaurants.", "Restaurant Menu", "/assets/templates/restaurant.jpg" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
