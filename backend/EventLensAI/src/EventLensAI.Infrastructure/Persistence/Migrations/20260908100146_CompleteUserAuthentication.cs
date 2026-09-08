using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations;

public partial class CompleteUserAuthentication : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData("roles", new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
            new object[] { new Guid("10000000-0000-0000-0000-000000000011"), new DateTime(2026,7,28,0,0,0,DateTimeKind.Utc), null!, false, "PlatformAdmin", null!, null! });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData("roles", "Id", new Guid("10000000-0000-0000-0000-000000000011"));
    }
}
