using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationToTemplateDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Add column UserId
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TemplateDrafts",
                type: "int",
                nullable: true);

            // 2️⃣ Create index
            migrationBuilder.CreateIndex(
                name: "IX_TemplateDrafts_UserId",
                table: "TemplateDrafts",
                column: "UserId");

            // 3️⃣ Add FK → Users(Id)
            migrationBuilder.AddForeignKey(
                name: "FK_TemplateDrafts_Users_UserId",
                table: "TemplateDrafts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the FK
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateDrafts_Users_UserId",
                table: "TemplateDrafts");

            // Remove index
            migrationBuilder.DropIndex(
                name: "IX_TemplateDrafts_UserId",
                table: "TemplateDrafts");

            // Remove the column
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TemplateDrafts");
        }
    }
}
