using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestBoothExperienceEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoothExperienceConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WelcomeMessage = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AttractTimeout = table.Column<int>(type: "int", nullable: false),
                    Theme = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoundEnabled = table.Column<bool>(type: "bit", nullable: false),
                    FullscreenEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AnimationType = table.Column<int>(type: "int", nullable: false),
                    BackgroundMedia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoothExperienceConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoothExperienceLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuestBoothSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromState = table.Column<int>(type: "int", nullable: false),
                    ToState = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoothExperienceLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuestBoothSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoothSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    SelectedMode = table.Column<int>(type: "int", nullable: true),
                    RecoveryToken = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestBoothSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoothExperienceConfigurations_EventId",
                table: "BoothExperienceConfigurations",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoothExperienceConfigurations_OrganizationId",
                table: "BoothExperienceConfigurations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothExperienceLogs_EventId",
                table: "BoothExperienceLogs",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothExperienceLogs_GuestBoothSessionId_OccurredAt",
                table: "BoothExperienceLogs",
                columns: new[] { "GuestBoothSessionId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BoothExperienceLogs_OrganizationId_CreatedAt",
                table: "BoothExperienceLogs",
                columns: new[] { "OrganizationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GuestBoothSessions_EventId",
                table: "GuestBoothSessions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestBoothSessions_OrganizationId_CreatedAt",
                table: "GuestBoothSessions",
                columns: new[] { "OrganizationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GuestBoothSessions_RecoveryToken",
                table: "GuestBoothSessions",
                column: "RecoveryToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoothExperienceConfigurations");

            migrationBuilder.DropTable(
                name: "BoothExperienceLogs");

            migrationBuilder.DropTable(
                name: "GuestBoothSessions");
        }
    }
}
