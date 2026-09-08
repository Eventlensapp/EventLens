using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCoreManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_organizations_OrganizationId",
                table: "events");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedManagerId",
                table: "events",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "events",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BrandProfileId",
                table: "events",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "events",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "events",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "events",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "events",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "events",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EventTypeId",
                table: "events",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "events",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "event_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_members_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_event_members_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "event_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsSystemType = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_types_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "event_types",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "Icon", "IsDeleted", "IsSystemType", "Name", "OrganizationId", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Wedding", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Birthday", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Corporate", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Graduation", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "School", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Exhibition", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Festival", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Brand Promotion", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Product Launch", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000010"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Conference", null, "Active", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000011"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, true, "Other", null, "Active", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_events_AssignedManagerId",
                table: "events",
                column: "AssignedManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_events_BranchId",
                table: "events",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_events_BrandProfileId",
                table: "events",
                column: "BrandProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_events_EventTypeId",
                table: "events",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_events_OrganizationId",
                table: "events",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_events_StartDate",
                table: "events",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_events_Status",
                table: "events",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_event_members_EventId",
                table: "event_members",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_members_EventId_UserId",
                table: "event_members",
                columns: new[] { "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_members_UserId",
                table: "event_members",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_OrganizationId_Name",
                table: "event_types",
                columns: new[] { "OrganizationId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_events_branches_BranchId",
                table: "events",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_brand_themes_BrandProfileId",
                table: "events",
                column: "BrandProfileId",
                principalTable: "brand_themes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_event_types_EventTypeId",
                table: "events",
                column: "EventTypeId",
                principalTable: "event_types",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_organizations_OrganizationId",
                table: "events",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_users_AssignedManagerId",
                table: "events",
                column: "AssignedManagerId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_branches_BranchId",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "FK_events_brand_themes_BrandProfileId",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "FK_events_event_types_EventTypeId",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "FK_events_organizations_OrganizationId",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "FK_events_users_AssignedManagerId",
                table: "events");

            migrationBuilder.DropTable(
                name: "event_members");

            migrationBuilder.DropTable(
                name: "event_types");

            migrationBuilder.DropIndex(
                name: "IX_events_AssignedManagerId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_BranchId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_BrandProfileId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_EventTypeId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_OrganizationId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_StartDate",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_Status",
                table: "events");

            migrationBuilder.DropColumn(
                name: "AssignedManagerId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "BrandProfileId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "City",
                table: "events");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "events");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "events");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "events");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "events");

            migrationBuilder.DropColumn(
                name: "EventTypeId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "events");

            migrationBuilder.AddForeignKey(
                name: "FK_events_organizations_OrganizationId",
                table: "events",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
