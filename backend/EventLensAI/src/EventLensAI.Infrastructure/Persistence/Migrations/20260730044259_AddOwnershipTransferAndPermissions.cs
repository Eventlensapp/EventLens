using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnershipTransferAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organization_ownership_transfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_ownership_transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_organization_ownership_transfers_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "organization_member_permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Allowed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_member_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_organization_member_permissions_organization_members_OrganizationMemberId",
                        column: x => x.OrganizationMemberId,
                        principalTable: "organization_members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_organization_member_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsDeleted", "Key", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manage organization settings", false, "organization.manage", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manage members and invitations", false, "organization.members.manage", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Create and manage events", false, "events.manage", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Capture and manage photos", false, "photos.manage", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manage templates", false, "templates.manage", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000006"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manage CRM and marketing", false, "crm.manage", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000007"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "View analytics", false, "analytics.view", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000008"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "View organization resources", false, "organization.view", null, null }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "PermissionId", "RoleId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000002"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000003"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000004"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000005"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000006"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000006"), new Guid("10000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000007"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000007"), new Guid("10000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000008"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000009"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000010"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000011"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000012"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000013"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000006"), new Guid("10000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000014"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000007"), new Guid("10000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000015"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000016"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000017"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000018"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000008"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000019"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000008"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000020"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000009"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000021"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000009"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000022"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000006"), new Guid("10000000-0000-0000-0000-000000000010"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000023"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000007"), new Guid("10000000-0000-0000-0000-000000000010"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000024"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000010"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000025"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000007"), new Guid("10000000-0000-0000-0000-000000000007"), null, null },
                    { new Guid("40000000-0000-0000-0000-000000000026"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("30000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000007"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_organization_member_permissions_OrganizationMemberId_PermissionId",
                table: "organization_member_permissions",
                columns: new[] { "OrganizationMemberId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_organization_member_permissions_PermissionId",
                table: "organization_member_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_organization_ownership_transfers_OrganizationId_Status",
                table: "organization_ownership_transfers",
                columns: new[] { "OrganizationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Key",
                table: "permissions",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_PermissionId",
                table: "role_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_RoleId_PermissionId",
                table: "role_permissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organization_member_permissions");

            migrationBuilder.DropTable(
                name: "organization_ownership_transfers");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "permissions");
        }
    }
}
