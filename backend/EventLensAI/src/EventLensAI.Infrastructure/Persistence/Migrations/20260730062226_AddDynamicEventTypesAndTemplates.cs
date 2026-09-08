using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicEventTypesAndTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "event_types",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "event_types",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "event_templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EventTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultBrandProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DefaultDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    ConfigurationJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemTemplate = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_templates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_templates_brand_themes_DefaultBrandProfileId",
                        column: x => x.DefaultBrandProfileId,
                        principalTable: "brand_themes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_templates_event_types_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "event_types",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_templates_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "event_template_usages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_template_usages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_template_usages_event_templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "event_templates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_template_usages_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.UpdateData(
                table: "event_types",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "Color", "IsActive" },
                values: new object[] { "#6C5CE7", true });

            migrationBuilder.CreateIndex(
                name: "IX_event_template_usages_EventId",
                table: "event_template_usages",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_template_usages_TemplateId",
                table: "event_template_usages",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_event_templates_DefaultBrandProfileId",
                table: "event_templates",
                column: "DefaultBrandProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_event_templates_EventTypeId",
                table: "event_templates",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_event_templates_OrganizationId",
                table: "event_templates",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_event_templates_OrganizationId_Name",
                table: "event_templates",
                columns: new[] { "OrganizationId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_template_usages");

            migrationBuilder.DropTable(
                name: "event_templates");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "event_types");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "event_types");
        }
    }
}
