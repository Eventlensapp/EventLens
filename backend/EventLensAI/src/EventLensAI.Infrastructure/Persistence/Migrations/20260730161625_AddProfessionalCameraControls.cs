using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalCameraControls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "camera_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CameraType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Zoom = table.Column<double>(type: "float", nullable: true),
                    Focus = table.Column<double>(type: "float", nullable: true),
                    Exposure = table.Column<double>(type: "float", nullable: true),
                    Brightness = table.Column<double>(type: "float", nullable: true),
                    Contrast = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    WhiteBalance = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TorchEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_camera_profiles_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "camera_settings_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CameraProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_settings_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_camera_settings_history_camera_profiles_CameraProfileId",
                        column: x => x.CameraProfileId,
                        principalTable: "camera_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_camera_settings_history_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_camera_settings_history_users_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_camera_profiles_OrganizationId_Name",
                table: "camera_profiles",
                columns: new[] { "OrganizationId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_camera_settings_history_CameraProfileId",
                table: "camera_settings_history",
                column: "CameraProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_settings_history_ChangedAt",
                table: "camera_settings_history",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_camera_settings_history_ChangedBy",
                table: "camera_settings_history",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_camera_settings_history_OrganizationId",
                table: "camera_settings_history",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "camera_settings_history");

            migrationBuilder.DropTable(
                name: "camera_profiles");
        }
    }
}
