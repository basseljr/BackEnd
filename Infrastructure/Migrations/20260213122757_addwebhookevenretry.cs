using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addwebhookevenretry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFailed",
                table: "PaymentWebhookEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRetryAt",
                table: "PaymentWebhookEvents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "PaymentWebhookEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFailed",
                table: "PaymentWebhookEvents");

            migrationBuilder.DropColumn(
                name: "LastRetryAt",
                table: "PaymentWebhookEvents");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "PaymentWebhookEvents");
        }
    }
}
