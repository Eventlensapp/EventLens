using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventLensAI.Infrastructure.Persistence.Migrations;

[DbContext(typeof(EventLensDbContext))]
[Migration("20260908173100_RepairIdentitySupportTables")]
public sealed class RepairIdentitySupportTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF OBJECT_ID(N'[dbo].[activity_logs]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[activity_logs] (
                    [Id] uniqueidentifier NOT NULL,
                    [UserId] uniqueidentifier NULL,
                    [Action] nvarchar(80) NOT NULL,
                    [Description] nvarchar(500) NOT NULL,
                    [IpAddress] nvarchar(64) NULL,
                    [Device] nvarchar(500) NULL,
                    [CreatedAt] datetime2 NOT NULL,
                    [UpdatedAt] datetime2 NULL,
                    [CreatedBy] uniqueidentifier NULL,
                    [UpdatedBy] uniqueidentifier NULL,
                    [IsDeleted] bit NOT NULL,
                    CONSTRAINT [PK_activity_logs] PRIMARY KEY ([Id])
                );
                CREATE INDEX [IX_activity_logs_UserId_CreatedAt] ON [dbo].[activity_logs] ([UserId], [CreatedAt]);
            END;

            IF OBJECT_ID(N'[dbo].[user_tokens]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[user_tokens] (
                    [Id] uniqueidentifier NOT NULL,
                    [UserId] uniqueidentifier NOT NULL,
                    [TokenHash] nvarchar(64) NOT NULL,
                    [Purpose] nvarchar(30) NOT NULL,
                    [ExpiresAt] datetime2 NOT NULL,
                    [UsedAt] datetime2 NULL,
                    [CreatedAt] datetime2 NOT NULL,
                    [UpdatedAt] datetime2 NULL,
                    [CreatedBy] uniqueidentifier NULL,
                    [UpdatedBy] uniqueidentifier NULL,
                    [IsDeleted] bit NOT NULL,
                    CONSTRAINT [PK_user_tokens] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_user_tokens_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[users] ([Id]) ON DELETE CASCADE
                );
                CREATE UNIQUE INDEX [IX_user_tokens_TokenHash] ON [dbo].[user_tokens] ([TokenHash]);
                CREATE INDEX [IX_user_tokens_UserId_Purpose] ON [dbo].[user_tokens] ([UserId], [Purpose]);
            END;

            IF OBJECT_ID(N'[dbo].[user_preferences]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[user_preferences] (
                    [Id] uniqueidentifier NOT NULL,
                    [UserId] uniqueidentifier NOT NULL,
                    [Theme] nvarchar(20) NOT NULL,
                    [Language] nvarchar(10) NOT NULL,
                    [EmailNotifications] bit NOT NULL,
                    [SecurityNotifications] bit NOT NULL,
                    [CreatedAt] datetime2 NOT NULL,
                    [UpdatedAt] datetime2 NULL,
                    [CreatedBy] uniqueidentifier NULL,
                    [UpdatedBy] uniqueidentifier NULL,
                    [IsDeleted] bit NOT NULL,
                    CONSTRAINT [PK_user_preferences] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_user_preferences_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[users] ([Id]) ON DELETE CASCADE
                );
                CREATE UNIQUE INDEX [IX_user_preferences_UserId] ON [dbo].[user_preferences] ([UserId]);
            END;

            IF OBJECT_ID(N'[dbo].[api_keys]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[api_keys] (
                    [Id] uniqueidentifier NOT NULL,
                    [UserId] uniqueidentifier NOT NULL,
                    [Name] nvarchar(100) NOT NULL,
                    [Prefix] nvarchar(20) NOT NULL,
                    [KeyHash] nvarchar(64) NOT NULL,
                    [ExpiresAt] datetime2 NULL,
                    [RevokedAt] datetime2 NULL,
                    [LastUsedAt] datetime2 NULL,
                    [CreatedAt] datetime2 NOT NULL,
                    [UpdatedAt] datetime2 NULL,
                    [CreatedBy] uniqueidentifier NULL,
                    [UpdatedBy] uniqueidentifier NULL,
                    [IsDeleted] bit NOT NULL,
                    CONSTRAINT [PK_api_keys] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_api_keys_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[users] ([Id]) ON DELETE CASCADE
                );
                CREATE UNIQUE INDEX [IX_api_keys_KeyHash] ON [dbo].[api_keys] ([KeyHash]);
                CREATE INDEX [IX_api_keys_UserId] ON [dbo].[api_keys] ([UserId]);
            END;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Corrective and intentionally irreversible to protect authentication and audit data.
    }
}
