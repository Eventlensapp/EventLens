using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventBrandingAndGuestExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageAssetId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AssetType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_assets_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "event_brand_configurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LogoAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BackgroundAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WatermarkAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    SecondaryColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    AccentColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    FontFamily = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThemeMode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ThemeOverridesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_brand_configurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_brand_configurations_brand_themes_BrandProfileId",
                        column: x => x.BrandProfileId,
                        principalTable: "brand_themes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_brand_configurations_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "event_experience_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WelcomeTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    WelcomeMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WelcomeImageAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CaptureMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletionMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GalleryTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GalleryDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EnableDownload = table.Column<bool>(type: "bit", nullable: false),
                    EnableSharing = table.Column<bool>(type: "bit", nullable: false),
                    ConfigurationJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_experience_settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_experience_settings_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "event_sponsors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LogoAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_sponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_sponsors_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_assets_EventId",
                table: "event_assets",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_brand_configurations_BrandProfileId",
                table: "event_brand_configurations",
                column: "BrandProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_event_brand_configurations_EventId",
                table: "event_brand_configurations",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_experience_settings_EventId",
                table: "event_experience_settings",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_sponsors_EventId",
                table: "event_sponsors",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_assets");

            migrationBuilder.DropTable(
                name: "event_brand_configurations");

            migrationBuilder.DropTable(
                name: "event_experience_settings");

            migrationBuilder.DropTable(
                name: "event_sponsors");
        }
    }
}
