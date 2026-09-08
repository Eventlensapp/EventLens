using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations;

[DbContext(typeof(EventLensDbContext))]
[Migration("20260908173000_RepairRefreshTokenSessionColumns")]
public sealed class RepairRefreshTokenSessionColumns : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF COL_LENGTH('dbo.refresh_tokens', 'DeviceInfo') IS NULL
                ALTER TABLE [dbo].[refresh_tokens] ADD [DeviceInfo] nvarchar(500) NULL;

            IF COL_LENGTH('dbo.refresh_tokens', 'IpAddress') IS NULL
                ALTER TABLE [dbo].[refresh_tokens] ADD [IpAddress] nvarchar(64) NULL;

            IF COL_LENGTH('dbo.refresh_tokens', 'LastActivityAt') IS NULL
                ALTER TABLE [dbo].[refresh_tokens] ADD [LastActivityAt] datetime2 NOT NULL
                    CONSTRAINT [DF_refresh_tokens_LastActivityAt] DEFAULT GETUTCDATE();
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // This repairs databases created by an incomplete earlier migration. It is intentionally
        // irreversible so a rollback cannot remove session data columns from a healthy database.
    }
}
