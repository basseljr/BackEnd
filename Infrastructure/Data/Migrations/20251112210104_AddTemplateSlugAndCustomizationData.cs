using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateSlugAndCustomizationData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomizationData",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CustomizationData", "Slug" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CustomizationData", "Slug" },
                values: new object[] { null, "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomizationData",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Templates");
        }
    }
}
