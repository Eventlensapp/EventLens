using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoCaptureEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "capture_configurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountdownDuration = table.Column<int>(type: "int", nullable: false),
                    NumberOfPhotos = table.Column<int>(type: "int", nullable: false),
                    CaptureInterval = table.Column<int>(type: "int", nullable: false),
                    ImageQuality = table.Column<decimal>(type: "decimal(4,3)", precision: 4, scale: 3, nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MirrorImage = table.Column<bool>(type: "bit", nullable: false),
                    AutoCaptureEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_capture_configurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_capture_configurations_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_capture_configurations_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "captured_photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaptureNumber = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessingStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_captured_photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_captured_photos_booth_sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "booth_sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_captured_photos_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_captured_photos_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_capture_configurations_EventId",
                table: "capture_configurations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_capture_configurations_OrganizationId_EventId",
                table: "capture_configurations",
                columns: new[] { "OrganizationId", "EventId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_captured_photos_CapturedAt",
                table: "captured_photos",
                column: "CapturedAt");

            migrationBuilder.CreateIndex(
                name: "IX_captured_photos_EventId",
                table: "captured_photos",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_captured_photos_OrganizationId",
                table: "captured_photos",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_captured_photos_ProcessingStatus",
                table: "captured_photos",
                column: "ProcessingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_captured_photos_SessionId",
                table: "captured_photos",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_captured_photos_SessionId_CaptureNumber",
                table: "captured_photos",
                columns: new[] { "SessionId", "CaptureNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "capture_configurations");

            migrationBuilder.DropTable(
                name: "captured_photos");
        }
    }
}
