using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsBusinessIntelligence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "analytics_daily_aggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhotographerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Metric = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    UniqueVisitors = table.Column<int>(type: "int", nullable: false),
                    LastAggregatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analytics_daily_aggregates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "analytics_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhotographerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Metric = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionKeyHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Browser = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    OperatingSystem = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DimensionsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analytics_records", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_analytics_daily_aggregates_EventId_Date_Metric",
                table: "analytics_daily_aggregates",
                columns: new[] { "EventId", "Date", "Metric" });

            migrationBuilder.CreateIndex(
                name: "IX_analytics_daily_aggregates_OrganizationId_Date_Metric_EventId_TemplateId_PhotographerId",
                table: "analytics_daily_aggregates",
                columns: new[] { "OrganizationId", "Date", "Metric", "EventId", "TemplateId", "PhotographerId" },
                unique: true,
                filter: "[EventId] IS NOT NULL AND [TemplateId] IS NOT NULL AND [PhotographerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_analytics_records_EventId_OccurredAt_Metric",
                table: "analytics_records",
                columns: new[] { "EventId", "OccurredAt", "Metric" });

            migrationBuilder.CreateIndex(
                name: "IX_analytics_records_OrganizationId_OccurredAt_Metric",
                table: "analytics_records",
                columns: new[] { "OrganizationId", "OccurredAt", "Metric" });

            migrationBuilder.CreateIndex(
                name: "IX_analytics_records_PhotoId_Metric",
                table: "analytics_records",
                columns: new[] { "PhotoId", "Metric" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analytics_daily_aggregates");

            migrationBuilder.DropTable(
                name: "analytics_records");
        }
    }
}
