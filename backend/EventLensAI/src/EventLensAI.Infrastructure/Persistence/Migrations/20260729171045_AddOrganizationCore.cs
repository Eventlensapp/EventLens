using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations;

/// <summary>Adds only the Organization Core type/status fields and lookup index.</summary>
public partial class AddOrganizationCore : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "OrganizationType", table: "organizations", type: "nvarchar(40)",
            maxLength: 40, nullable: false, defaultValue: "Other");
        migrationBuilder.AddColumn<string>(
            name: "Status", table: "organizations", type: "nvarchar(20)",
            maxLength: 20, nullable: false, defaultValue: "Active");
        migrationBuilder.AddColumn<string>(
            name: "Status", table: "organization_members", type: "nvarchar(20)",
            maxLength: 20, nullable: false, defaultValue: "Active");
        migrationBuilder.CreateIndex(
            name: "IX_organization_members_OrganizationId",
            table: "organization_members", column: "OrganizationId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_organization_members_OrganizationId", table: "organization_members");
        migrationBuilder.DropColumn(name: "OrganizationType", table: "organizations");
        migrationBuilder.DropColumn(name: "Status", table: "organizations");
        migrationBuilder.DropColumn(name: "Status", table: "organization_members");
    }
}
