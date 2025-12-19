using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantCustomizationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantCustomizations_Tenants_TenantId",
                table: "TenantCustomizations");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantCustomizations_Tenants_TenantId",
                table: "TenantCustomizations",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantCustomizations_Tenants_TenantId",
                table: "TenantCustomizations");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantCustomizations_Tenants_TenantId",
                table: "TenantCustomizations",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
