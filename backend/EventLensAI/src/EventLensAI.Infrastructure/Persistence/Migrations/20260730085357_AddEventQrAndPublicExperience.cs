using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventQrAndPublicExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_access_configurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPublicEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RequirePassword = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AllowGuestAccess = table.Column<bool>(type: "bit", nullable: false),
                    AllowGalleryAccess = table.Column<bool>(type: "bit", nullable: false),
                    AllowDownloads = table.Column<bool>(type: "bit", nullable: false),
                    AllowSharing = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_access_configurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_access_configurations_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "event_qr_codes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QRType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PublicUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageStoragePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CustomizationJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_qr_codes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_qr_codes_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "guest_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeviceInfo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guest_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guest_sessions_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "event_access_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QRCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_access_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_access_logs_event_qr_codes_QRCodeId",
                        column: x => x.QRCodeId,
                        principalTable: "event_qr_codes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_access_logs_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_access_logs_guest_sessions_GuestSessionId",
                        column: x => x.GuestSessionId,
                        principalTable: "guest_sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_access_configurations_EventId",
                table: "event_access_configurations",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_access_logs_EventId",
                table: "event_access_logs",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_access_logs_GuestSessionId",
                table: "event_access_logs",
                column: "GuestSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_event_access_logs_QRCodeId",
                table: "event_access_logs",
                column: "QRCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_event_access_logs_Timestamp",
                table: "event_access_logs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_event_qr_codes_EventId",
                table: "event_qr_codes",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_qr_codes_Token",
                table: "event_qr_codes",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guest_sessions_EventId",
                table: "guest_sessions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_guest_sessions_SessionToken",
                table: "guest_sessions",
                column: "SessionToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_access_configurations");

            migrationBuilder.DropTable(
                name: "event_access_logs");

            migrationBuilder.DropTable(
                name: "event_qr_codes");

            migrationBuilder.DropTable(
                name: "guest_sessions");
        }
    }
}
