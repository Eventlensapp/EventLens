using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBoothSessionEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BoothId",
                table: "booth_sessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceInformation",
                table: "booth_sessions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedAt",
                table: "booth_sessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuestEmail",
                table: "booth_sessions",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuestName",
                table: "booth_sessions",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityAt",
                table: "booth_sessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MetadataJson",
                table: "booth_sessions",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "booth_sessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionToken",
                table: "booth_sessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionType",
                table: "booth_sessions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "booth_session_activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booth_session_activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_booth_session_activities_booth_sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "booth_sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_booth_sessions_OrganizationId",
                table: "booth_sessions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_booth_sessions_SessionToken",
                table: "booth_sessions",
                column: "SessionToken",
                unique: true,
                filter: "[SessionToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_booth_sessions_StartedAt",
                table: "booth_sessions",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_booth_sessions_Status",
                table: "booth_sessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_booth_session_activities_SessionId_Timestamp",
                table: "booth_session_activities",
                columns: new[] { "SessionId", "Timestamp" });

            migrationBuilder.AddForeignKey(
                name: "FK_booth_sessions_organizations_OrganizationId",
                table: "booth_sessions",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booth_sessions_organizations_OrganizationId",
                table: "booth_sessions");

            migrationBuilder.DropTable(
                name: "booth_session_activities");

            migrationBuilder.DropIndex(
                name: "IX_booth_sessions_OrganizationId",
                table: "booth_sessions");

            migrationBuilder.DropIndex(
                name: "IX_booth_sessions_SessionToken",
                table: "booth_sessions");

            migrationBuilder.DropIndex(
                name: "IX_booth_sessions_StartedAt",
                table: "booth_sessions");

            migrationBuilder.DropIndex(
                name: "IX_booth_sessions_Status",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "BoothId",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "DeviceInformation",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "EndedAt",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "GuestName",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "LastActivityAt",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "MetadataJson",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "SessionToken",
                table: "booth_sessions");

            migrationBuilder.DropColumn(
                name: "SessionType",
                table: "booth_sessions");
        }
    }
}
