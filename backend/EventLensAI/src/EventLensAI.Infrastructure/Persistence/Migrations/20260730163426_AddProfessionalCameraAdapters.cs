using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalCameraAdapters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "professional_cameras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ConnectionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FirmwareVersion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastConnectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastCommunicationAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professional_cameras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_professional_cameras_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "camera_adapters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionalCameraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AdapterKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_adapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_camera_adapters_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_camera_adapters_professional_cameras_ProfessionalCameraId",
                        column: x => x.ProfessionalCameraId,
                        principalTable: "professional_cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "camera_capability_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionalCameraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SupportedModes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MaxISO = table.Column<int>(type: "int", nullable: true),
                    ShutterSpeed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Aperture = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FocusModes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FlashSupport = table.Column<bool>(type: "bit", nullable: false),
                    VideoSupport = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_capability_profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_camera_capability_profiles_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_camera_capability_profiles_professional_cameras_ProfessionalCameraId",
                        column: x => x.ProfessionalCameraId,
                        principalTable: "professional_cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "camera_connection_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionalCameraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_connection_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_camera_connection_logs_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_camera_connection_logs_professional_cameras_ProfessionalCameraId",
                        column: x => x.ProfessionalCameraId,
                        principalTable: "professional_cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "camera_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionalCameraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoothSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_camera_events_booth_sessions_BoothSessionId",
                        column: x => x.BoothSessionId,
                        principalTable: "booth_sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_camera_events_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_camera_events_professional_cameras_ProfessionalCameraId",
                        column: x => x.ProfessionalCameraId,
                        principalTable: "professional_cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_camera_adapters_OrganizationId_ProfessionalCameraId",
                table: "camera_adapters",
                columns: new[] { "OrganizationId", "ProfessionalCameraId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_camera_adapters_ProfessionalCameraId",
                table: "camera_adapters",
                column: "ProfessionalCameraId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_capability_profiles_OrganizationId_ProfessionalCameraId",
                table: "camera_capability_profiles",
                columns: new[] { "OrganizationId", "ProfessionalCameraId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_camera_capability_profiles_ProfessionalCameraId",
                table: "camera_capability_profiles",
                column: "ProfessionalCameraId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_connection_logs_OccurredAt",
                table: "camera_connection_logs",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_camera_connection_logs_OrganizationId",
                table: "camera_connection_logs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_connection_logs_ProfessionalCameraId",
                table: "camera_connection_logs",
                column: "ProfessionalCameraId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_events_BoothSessionId",
                table: "camera_events",
                column: "BoothSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_events_OccurredAt",
                table: "camera_events",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_camera_events_OrganizationId",
                table: "camera_events",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_events_ProfessionalCameraId",
                table: "camera_events",
                column: "ProfessionalCameraId");

            migrationBuilder.CreateIndex(
                name: "IX_professional_cameras_LastConnectedAt",
                table: "professional_cameras",
                column: "LastConnectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_professional_cameras_OrganizationId",
                table: "professional_cameras",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_professional_cameras_ProviderType",
                table: "professional_cameras",
                column: "ProviderType");

            migrationBuilder.CreateIndex(
                name: "IX_professional_cameras_Status",
                table: "professional_cameras",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "camera_adapters");

            migrationBuilder.DropTable(
                name: "camera_capability_profiles");

            migrationBuilder.DropTable(
                name: "camera_connection_logs");

            migrationBuilder.DropTable(
                name: "camera_events");

            migrationBuilder.DropTable(
                name: "professional_cameras");
        }
    }
}
