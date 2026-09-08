using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationTeamManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvitedBy",
                table: "organization_members",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "organization_invitations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000008"), new DateTime(2026, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "BoothOperator", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new DateTime(2026, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Designer", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000010"), new DateTime(2026, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "MarketingManager", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_organization_invitations_Email",
                table: "organization_invitations",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_organization_invitations_Email",
                table: "organization_invitations");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"));

            migrationBuilder.DropColumn(
                name: "InvitedBy",
                table: "organization_members");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "organization_invitations");
        }
    }
}
