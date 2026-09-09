using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTemporaryPasswordRequirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('users', 'MustChangePassword') IS NULL
                    ALTER TABLE [users] ADD [MustChangePassword] bit NOT NULL DEFAULT CAST(0 AS bit);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The column may have been created by a previous schema repair, so retain it on rollback.
        }
    }
}
