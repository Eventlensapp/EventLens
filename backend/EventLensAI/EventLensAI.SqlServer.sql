IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [ai_prompt_definitions] (
        [Id] uniqueidentifier NOT NULL,
        [Key] nvarchar(100) NOT NULL,
        [JobType] nvarchar(50) NOT NULL,
        [Prompt] nvarchar(4000) NOT NULL,
        [NegativePrompt] nvarchar(2000) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ai_prompt_definitions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [audit_logs] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [ResourceType] nvarchar(100) NOT NULL,
        [ResourceId] uniqueidentifier NOT NULL,
        [Action] nvarchar(30) NOT NULL,
        [IPAddress] nvarchar(64) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_audit_logs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [organizations] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Slug] nvarchar(100) NOT NULL,
        [Description] nvarchar(4000) NULL,
        [Logo] nvarchar(2048) NULL,
        [Website] nvarchar(2048) NULL,
        [Email] nvarchar(320) NULL,
        [Phone] nvarchar(30) NULL,
        [Address] nvarchar(1000) NULL,
        [Country] nvarchar(100) NULL,
        [TimeZone] nvarchar(100) NOT NULL,
        [PrimaryColor] nvarchar(7) NOT NULL,
        [SecondaryColor] nvarchar(7) NOT NULL,
        [Plan] nvarchar(30) NOT NULL,
        [StorageUsed] bigint NOT NULL,
        [StorageLimit] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_organizations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [roles] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [users] (
        [Id] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(320) NOT NULL,
        [NormalizedEmail] nvarchar(320) NOT NULL,
        [PasswordHash] nvarchar(512) NOT NULL,
        [Phone] nvarchar(30) NULL,
        [ProfileImage] nvarchar(2048) NULL,
        [IsActive] bit NOT NULL,
        [LastLoginAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [ai_backgrounds] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NULL,
        [Name] nvarchar(200) NOT NULL,
        [Category] nvarchar(50) NOT NULL,
        [ImageUrl] nvarchar(2048) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ai_backgrounds] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ai_backgrounds_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [organizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [events] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Slug] nvarchar(120) NOT NULL,
        [Description] nvarchar(4000) NULL,
        [EventType] nvarchar(40) NOT NULL,
        [Venue] nvarchar(500) NULL,
        [Address] nvarchar(1000) NULL,
        [Latitude] decimal(18,2) NULL,
        [Longitude] decimal(18,2) NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [CoverImage] nvarchar(max) NULL,
        [Logo] nvarchar(2048) NULL,
        [PrimaryColor] nvarchar(7) NOT NULL,
        [SecondaryColor] nvarchar(7) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [GuestLimit] int NULL,
        [PhotoLimit] int NULL,
        [StorageLimit] bigint NULL,
        [PublicGalleryEnabled] bit NOT NULL,
        [RequireGuestRegistration] bit NOT NULL,
        [AllowDownloads] bit NOT NULL,
        [AllowSocialSharing] bit NOT NULL,
        [EnableAI] bit NOT NULL,
        [EnableQRCode] bit NOT NULL,
        [PublicUrl] nvarchar(2048) NULL,
        [QRCodeUrl] nvarchar(2048) NULL,
        [QRCodeSvg] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_events] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_events_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [organizations] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [subscriptions] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Plan] nvarchar(30) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_subscriptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_subscriptions_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [organizations] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [templates] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NULL,
        [Name] nvarchar(200) NOT NULL,
        [Category] nvarchar(100) NOT NULL,
        [PreviewImage] nvarchar(2048) NULL,
        [ConfigurationJson] nvarchar(max) NOT NULL,
        [IsPremium] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_templates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_templates_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [organizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [organization_invitations] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Email] nvarchar(320) NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [TokenHash] nvarchar(64) NOT NULL,
        [InvitedBy] uniqueidentifier NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [AcceptedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_organization_invitations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_organization_invitations_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [organizations] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_organization_invitations_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [roles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [organization_members] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [JoinedAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_organization_members] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_organization_members_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [organizations] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_organization_members_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [roles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_organization_members_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [refresh_tokens] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TokenHash] nvarchar(64) NOT NULL,
        [FamilyId] uniqueidentifier NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [RevokedAt] datetime2 NULL,
        [ReplacedByTokenId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_refresh_tokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_refresh_tokens_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [ai_jobs] (
        [Id] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NOT NULL,
        [SessionId] uniqueidentifier NULL,
        [PhotoId] uniqueidentifier NULL,
        [JobType] nvarchar(50) NOT NULL,
        [Provider] nvarchar(100) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [Progress] int NOT NULL,
        [Prompt] nvarchar(4000) NULL,
        [NegativePrompt] nvarchar(2000) NULL,
        [InputImage] nvarchar(2048) NOT NULL,
        [OutputImage] nvarchar(2048) NULL,
        [StartedAt] datetime2 NULL,
        [CompletedAt] datetime2 NULL,
        [ErrorMessage] nvarchar(2000) NULL,
        [RetryCount] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ai_jobs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ai_jobs_events_EventId] FOREIGN KEY ([EventId]) REFERENCES [events] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [booth_sessions] (
        [Id] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NOT NULL,
        [GuestId] uniqueidentifier NULL,
        [Status] nvarchar(30) NOT NULL,
        [StartedAt] datetime2 NOT NULL,
        [CompletedAt] datetime2 NULL,
        [CaptureMode] nvarchar(30) NOT NULL,
        [PhotoCount] int NOT NULL,
        [Countdown] int NOT NULL,
        [TemplateId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_booth_sessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_booth_sessions_events_EventId] FOREIGN KEY ([EventId]) REFERENCES [events] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [event_branding] (
        [Id] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NOT NULL,
        [Logo] nvarchar(2048) NULL,
        [Watermark] nvarchar(2048) NULL,
        [Background] nvarchar(2048) NULL,
        [SplashScreen] nvarchar(2048) NULL,
        [BrandFontsJson] nvarchar(max) NULL,
        [BrandColorsJson] nvarchar(max) NULL,
        [CustomCss] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_event_branding] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_event_branding_events_EventId] FOREIGN KEY ([EventId]) REFERENCES [events] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [event_settings] (
        [Id] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NOT NULL,
        [Countdown] int NOT NULL,
        [CaptureMode] nvarchar(30) NOT NULL,
        [TemplateId] uniqueidentifier NULL,
        [Language] nvarchar(10) NOT NULL,
        [Watermark] nvarchar(2048) NULL,
        [DefaultFilter] nvarchar(100) NULL,
        [PrintEnabled] bit NOT NULL,
        [GIFEnabled] bit NOT NULL,
        [BoomerangEnabled] bit NOT NULL,
        [VideoEnabled] bit NOT NULL,
        [AIEnabled] bit NOT NULL,
        [BackgroundRemovalEnabled] bit NOT NULL,
        [FaceDetectionEnabled] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_event_settings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_event_settings_events_EventId] FOREIGN KEY ([EventId]) REFERENCES [events] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [galleries] (
        [Id] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NOT NULL,
        [PublicUrl] nvarchar(2048) NOT NULL,
        [QRCode] nvarchar(2048) NOT NULL,
        [IsPrivate] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_galleries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_galleries_events_EventId] FOREIGN KEY ([EventId]) REFERENCES [events] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE TABLE [photos] (
        [Id] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NOT NULL,
        [SessionId] uniqueidentifier NULL,
        [OriginalImageUrl] nvarchar(2048) NOT NULL,
        [ProcessedImageUrl] nvarchar(2048) NULL,
        [ThumbnailUrl] nvarchar(2048) NULL,
        [Width] int NOT NULL,
        [Height] int NOT NULL,
        [FileSize] bigint NOT NULL,
        [Format] nvarchar(20) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [ProcessingType] nvarchar(30) NOT NULL,
        [FailureReason] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_photos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_photos_booth_sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [booth_sessions] ([Id]),
        CONSTRAINT [FK_photos_events_EventId] FOREIGN KEY ([EventId]) REFERENCES [events] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[roles]'))
        SET IDENTITY_INSERT [roles] ON;
    EXEC(N'INSERT INTO [roles] ([Id], [CreatedAt], [CreatedBy], [IsDeleted], [Name], [UpdatedAt], [UpdatedBy])
    VALUES (''10000000-0000-0000-0000-000000000001'', ''2026-07-28T00:00:00.0000000Z'', NULL, CAST(0 AS bit), N''SuperAdmin'', NULL, NULL),
    (''10000000-0000-0000-0000-000000000002'', ''2026-07-28T00:00:00.0000000Z'', NULL, CAST(0 AS bit), N''Owner'', NULL, NULL),
    (''10000000-0000-0000-0000-000000000003'', ''2026-07-28T00:00:00.0000000Z'', NULL, CAST(0 AS bit), N''Manager'', NULL, NULL),
    (''10000000-0000-0000-0000-000000000004'', ''2026-07-28T00:00:00.0000000Z'', NULL, CAST(0 AS bit), N''Photographer'', NULL, NULL),
    (''10000000-0000-0000-0000-000000000005'', ''2026-07-28T00:00:00.0000000Z'', NULL, CAST(0 AS bit), N''Guest'', NULL, NULL),
    (''10000000-0000-0000-0000-000000000006'', ''2026-07-28T00:00:00.0000000Z'', NULL, CAST(0 AS bit), N''Editor'', NULL, NULL),
    (''10000000-0000-0000-0000-000000000007'', ''2026-07-28T00:00:00.0000000Z'', NULL, CAST(0 AS bit), N''Viewer'', NULL, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[roles]'))
        SET IDENTITY_INSERT [roles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_ai_backgrounds_OrganizationId_Category] ON [ai_backgrounds] ([OrganizationId], [Category]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_ai_jobs_EventId_Status_CreatedAt] ON [ai_jobs] ([EventId], [Status], [CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ai_prompt_definitions_Key_JobType] ON [ai_prompt_definitions] ([Key], [JobType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_audit_logs_ResourceType_ResourceId_CreatedAt] ON [audit_logs] ([ResourceType], [ResourceId], [CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_booth_sessions_EventId_StartedAt] ON [booth_sessions] ([EventId], [StartedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_event_branding_EventId] ON [event_branding] ([EventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_event_settings_EventId] ON [event_settings] ([EventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_events_OrganizationId_Slug] ON [events] ([OrganizationId], [Slug]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_galleries_EventId] ON [galleries] ([EventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_galleries_PublicUrl] ON [galleries] ([PublicUrl]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_organization_invitations_OrganizationId_Email] ON [organization_invitations] ([OrganizationId], [Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_organization_invitations_RoleId] ON [organization_invitations] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_organization_invitations_TokenHash] ON [organization_invitations] ([TokenHash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_organization_members_OrganizationId_UserId] ON [organization_members] ([OrganizationId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_organization_members_RoleId] ON [organization_members] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_organization_members_UserId] ON [organization_members] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_organizations_Slug] ON [organizations] ([Slug]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_photos_EventId] ON [photos] ([EventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_photos_SessionId] ON [photos] ([SessionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_refresh_tokens_TokenHash] ON [refresh_tokens] ([TokenHash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_refresh_tokens_UserId_FamilyId] ON [refresh_tokens] ([UserId], [FamilyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_roles_Name] ON [roles] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_subscriptions_OrganizationId] ON [subscriptions] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_templates_OrganizationId_Name] ON [templates] ([OrganizationId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_users_NormalizedEmail] ON [users] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728122023_InitialSqlServer'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260728122023_InitialSqlServer', N'9.0.18');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [automations] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Trigger] int NOT NULL,
        [ConditionsJson] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_automations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [campaign_logs] (
        [Id] uniqueidentifier NOT NULL,
        [CampaignId] uniqueidentifier NOT NULL,
        [GuestId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [SentAt] datetime2 NULL,
        [ProviderMessageId] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_campaign_logs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [campaigns] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [SegmentId] uniqueidentifier NULL,
        [Name] nvarchar(max) NOT NULL,
        [Channel] int NOT NULL,
        [Status] int NOT NULL,
        [ContentJson] nvarchar(max) NOT NULL,
        [ScheduledAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_campaigns] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [coupons] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Code] nvarchar(450) NOT NULL,
        [DiscountType] int NOT NULL,
        [Value] decimal(18,2) NOT NULL,
        [Expiry] datetime2 NOT NULL,
        [UsageLimit] int NOT NULL,
        [UsageCount] int NOT NULL,
        [OnePerGuest] bit NOT NULL,
        [IsReferral] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_coupons] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [crm_tags] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Color] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_crm_tags] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [emails] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [GuestId] uniqueidentifier NOT NULL,
        [CampaignId] uniqueidentifier NULL,
        [Destination] nvarchar(320) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [ProviderMessageId] nvarchar(max) NULL,
        CONSTRAINT [PK_emails] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [guests] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(320) NULL,
        [Phone] nvarchar(50) NULL,
        [Company] nvarchar(200) NULL,
        [JobTitle] nvarchar(150) NULL,
        [Country] nvarchar(100) NULL,
        [City] nvarchar(100) NULL,
        [BirthDate] date NULL,
        [Gender] nvarchar(max) NULL,
        [CustomFieldsJson] nvarchar(max) NOT NULL,
        [MarketingConsent] bit NOT NULL,
        [ConsentVersion] nvarchar(max) NULL,
        [ConsentDate] datetime2 NULL,
        [ConsentIpAddress] nvarchar(max) NULL,
        [PrivacyPolicyAccepted] bit NOT NULL,
        [LastVisit] datetime2 NOT NULL,
        [TotalEvents] int NOT NULL,
        [LifetimeValue] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_guests] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [lead_forms] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NULL,
        [Name] nvarchar(200) NOT NULL,
        [SchemaJson] nvarchar(max) NOT NULL,
        [Language] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_lead_forms] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [referrals] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [ReferrerGuestId] uniqueidentifier NOT NULL,
        [ReferredGuestId] uniqueidentifier NULL,
        [Code] nvarchar(450) NOT NULL,
        [Invites] int NOT NULL,
        [Registrations] int NOT NULL,
        [Conversions] int NOT NULL,
        [RewardJson] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_referrals] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [segments] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [RulesJson] nvarchar(max) NOT NULL,
        [IsDynamic] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_segments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [sms] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [GuestId] uniqueidentifier NOT NULL,
        [CampaignId] uniqueidentifier NULL,
        [Destination] nvarchar(320) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [ProviderMessageId] nvarchar(max) NULL,
        CONSTRAINT [PK_sms] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [survey_responses] (
        [Id] uniqueidentifier NOT NULL,
        [SurveyId] uniqueidentifier NOT NULL,
        [GuestId] uniqueidentifier NOT NULL,
        [AnswersJson] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_survey_responses] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [surveys] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NULL,
        [Name] nvarchar(max) NOT NULL,
        [QuestionsJson] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_surveys] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [whatsapp_messages] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [GuestId] uniqueidentifier NOT NULL,
        [CampaignId] uniqueidentifier NULL,
        [Destination] nvarchar(320) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [ProviderMessageId] nvarchar(max) NULL,
        CONSTRAINT [PK_whatsapp_messages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [workflow_steps] (
        [Id] uniqueidentifier NOT NULL,
        [AutomationId] uniqueidentifier NOT NULL,
        [Order] int NOT NULL,
        [Action] int NOT NULL,
        [ConfigurationJson] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_workflow_steps] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [guest_activities] (
        [Id] uniqueidentifier NOT NULL,
        [GuestId] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NULL,
        [Type] int NOT NULL,
        [DataJson] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_guest_activities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_guest_activities_guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [guests] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [guest_check_ins] (
        [Id] uniqueidentifier NOT NULL,
        [GuestId] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NOT NULL,
        [Method] int NOT NULL,
        [CheckedInAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_guest_check_ins] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_guest_check_ins_events_EventId] FOREIGN KEY ([EventId]) REFERENCES [events] ([Id]),
        CONSTRAINT [FK_guest_check_ins_guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [guests] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [guest_tags] (
        [GuestId] uniqueidentifier NOT NULL,
        [TagId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_guest_tags] PRIMARY KEY ([GuestId], [TagId]),
        CONSTRAINT [FK_guest_tags_crm_tags_TagId] FOREIGN KEY ([TagId]) REFERENCES [crm_tags] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_guest_tags_guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [guests] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE TABLE [lead_responses] (
        [Id] uniqueidentifier NOT NULL,
        [LeadFormId] uniqueidentifier NOT NULL,
        [GuestId] uniqueidentifier NOT NULL,
        [ValuesJson] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_lead_responses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_lead_responses_guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [guests] ([Id]),
        CONSTRAINT [FK_lead_responses_lead_forms_LeadFormId] FOREIGN KEY ([LeadFormId]) REFERENCES [lead_forms] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_campaign_logs_CampaignId_GuestId] ON [campaign_logs] ([CampaignId], [GuestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_campaigns_OrganizationId_Status] ON [campaigns] ([OrganizationId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE UNIQUE INDEX [IX_coupons_OrganizationId_Code] ON [coupons] ([OrganizationId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE UNIQUE INDEX [IX_crm_tags_OrganizationId_Name] ON [crm_tags] ([OrganizationId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_emails_OrganizationId_Status] ON [emails] ([OrganizationId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_guest_activities_GuestId_CreatedAt] ON [guest_activities] ([GuestId], [CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE UNIQUE INDEX [IX_guest_check_ins_EventId_GuestId] ON [guest_check_ins] ([EventId], [GuestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_guest_check_ins_GuestId] ON [guest_check_ins] ([GuestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_guest_tags_TagId] ON [guest_tags] ([TagId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_guests_OrganizationId_Email] ON [guests] ([OrganizationId], [Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_lead_forms_OrganizationId_EventId] ON [lead_forms] ([OrganizationId], [EventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_lead_responses_GuestId] ON [lead_responses] ([GuestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_lead_responses_LeadFormId] ON [lead_responses] ([LeadFormId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE UNIQUE INDEX [IX_referrals_Code] ON [referrals] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_sms_OrganizationId_Status] ON [sms] ([OrganizationId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE INDEX [IX_whatsapp_messages_OrganizationId_Status] ON [whatsapp_messages] ([OrganizationId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    CREATE UNIQUE INDEX [IX_workflow_steps_AutomationId_Order] ON [workflow_steps] ([AutomationId], [Order]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728143250_AddCrmMarketingIntelligence'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260728143250_AddCrmMarketingIntelligence', N'9.0.18');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728153852_AddAnalyticsBusinessIntelligence'
)
BEGIN
    CREATE TABLE [analytics_daily_aggregates] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NULL,
        [TemplateId] uniqueidentifier NULL,
        [PhotographerId] uniqueidentifier NULL,
        [Date] date NOT NULL,
        [Metric] nvarchar(50) NOT NULL,
        [Total] int NOT NULL,
        [UniqueVisitors] int NOT NULL,
        [LastAggregatedAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_analytics_daily_aggregates] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728153852_AddAnalyticsBusinessIntelligence'
)
BEGIN
    CREATE TABLE [analytics_records] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NULL,
        [GuestId] uniqueidentifier NULL,
        [PhotoId] uniqueidentifier NULL,
        [TemplateId] uniqueidentifier NULL,
        [PhotographerId] uniqueidentifier NULL,
        [Metric] nvarchar(50) NOT NULL,
        [Value] int NOT NULL,
        [OccurredAt] datetime2 NOT NULL,
        [SessionKeyHash] nvarchar(64) NULL,
        [DeviceType] nvarchar(20) NOT NULL,
        [Browser] nvarchar(80) NULL,
        [OperatingSystem] nvarchar(80) NULL,
        [Country] nvarchar(100) NULL,
        [City] nvarchar(100) NULL,
        [DimensionsJson] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_analytics_records] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728153852_AddAnalyticsBusinessIntelligence'
)
BEGIN
    CREATE INDEX [IX_analytics_daily_aggregates_EventId_Date_Metric] ON [analytics_daily_aggregates] ([EventId], [Date], [Metric]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728153852_AddAnalyticsBusinessIntelligence'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_analytics_daily_aggregates_OrganizationId_Date_Metric_EventId_TemplateId_PhotographerId] ON [analytics_daily_aggregates] ([OrganizationId], [Date], [Metric], [EventId], [TemplateId], [PhotographerId]) WHERE [EventId] IS NOT NULL AND [TemplateId] IS NOT NULL AND [PhotographerId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728153852_AddAnalyticsBusinessIntelligence'
)
BEGIN
    CREATE INDEX [IX_analytics_records_EventId_OccurredAt_Metric] ON [analytics_records] ([EventId], [OccurredAt], [Metric]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728153852_AddAnalyticsBusinessIntelligence'
)
BEGIN
    CREATE INDEX [IX_analytics_records_OrganizationId_OccurredAt_Metric] ON [analytics_records] ([OrganizationId], [OccurredAt], [Metric]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728153852_AddAnalyticsBusinessIntelligence'
)
BEGIN
    CREATE INDEX [IX_analytics_records_PhotoId_Metric] ON [analytics_records] ([PhotoId], [Metric]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728153852_AddAnalyticsBusinessIntelligence'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260728153852_AddAnalyticsBusinessIntelligence', N'9.0.18');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    ALTER TABLE [subscriptions] ADD [BillingInterval] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    ALTER TABLE [subscriptions] ADD [CancelAtPeriodEnd] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    ALTER TABLE [subscriptions] ADD [CancelledAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    ALTER TABLE [subscriptions] ADD [GracePeriodEndsAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    ALTER TABLE [subscriptions] ADD [Provider] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    ALTER TABLE [subscriptions] ADD [ProviderSubscriptionId] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    ALTER TABLE [subscriptions] ADD [TrialEndsAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE TABLE [billing_coupons] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NULL,
        [Code] nvarchar(50) NOT NULL,
        [DiscountType] nvarchar(30) NOT NULL,
        [Value] decimal(18,2) NOT NULL,
        [Expiry] datetime2 NOT NULL,
        [UsageLimit] int NOT NULL,
        [UsageCount] int NOT NULL,
        [IsReferral] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_billing_coupons] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE TABLE [billing_plans] (
        [Id] uniqueidentifier NOT NULL,
        [Plan] nvarchar(30) NOT NULL,
        [Name] nvarchar(80) NOT NULL,
        [MonthlyPrice] decimal(18,2) NOT NULL,
        [YearlyPrice] decimal(18,2) NOT NULL,
        [Currency] nvarchar(3) NOT NULL,
        [MaximumOrganizations] int NOT NULL,
        [MaximumEvents] int NOT NULL,
        [MaximumTeamMembers] int NOT NULL,
        [MonthlyAICredits] int NOT NULL,
        [StorageLimit] bigint NOT NULL,
        [GalleryLimit] int NOT NULL,
        [TemplateAccess] nvarchar(30) NOT NULL,
        [CustomBranding] bit NOT NULL,
        [WhiteLabel] bit NOT NULL,
        [ApiAccess] bit NOT NULL,
        [CustomDomains] bit NOT NULL,
        [PrioritySupport] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_billing_plans] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE TABLE [billing_webhooks] (
        [Id] uniqueidentifier NOT NULL,
        [Provider] nvarchar(50) NOT NULL,
        [ExternalId] nvarchar(200) NOT NULL,
        [Type] nvarchar(50) NOT NULL,
        [PayloadHash] nvarchar(64) NOT NULL,
        [ReceivedAt] datetime2 NOT NULL,
        [ProcessedAt] datetime2 NULL,
        [Error] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_billing_webhooks] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE TABLE [invoices] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [SubscriptionId] uniqueidentifier NOT NULL,
        [InvoiceNumber] nvarchar(50) NOT NULL,
        [Customer] nvarchar(200) NOT NULL,
        [Plan] nvarchar(30) NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Tax] decimal(18,2) NOT NULL,
        [Discount] decimal(18,2) NOT NULL,
        [Currency] nvarchar(3) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [IssueDate] datetime2 NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_invoices] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE TABLE [payment_transactions] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [InvoiceId] uniqueidentifier NULL,
        [Provider] nvarchar(50) NOT NULL,
        [ProviderReference] nvarchar(200) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Currency] nvarchar(3) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [RefundedAmount] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_payment_transactions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE TABLE [usage_counters] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Metric] nvarchar(40) NOT NULL,
        [PeriodStart] date NOT NULL,
        [Quantity] bigint NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_usage_counters] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ApiAccess', N'CreatedAt', N'CreatedBy', N'Currency', N'CustomBranding', N'CustomDomains', N'GalleryLimit', N'IsActive', N'IsDeleted', N'MaximumEvents', N'MaximumOrganizations', N'MaximumTeamMembers', N'MonthlyAICredits', N'MonthlyPrice', N'Name', N'Plan', N'PrioritySupport', N'StorageLimit', N'TemplateAccess', N'UpdatedAt', N'UpdatedBy', N'WhiteLabel', N'YearlyPrice') AND [object_id] = OBJECT_ID(N'[billing_plans]'))
        SET IDENTITY_INSERT [billing_plans] ON;
    EXEC(N'INSERT INTO [billing_plans] ([Id], [ApiAccess], [CreatedAt], [CreatedBy], [Currency], [CustomBranding], [CustomDomains], [GalleryLimit], [IsActive], [IsDeleted], [MaximumEvents], [MaximumOrganizations], [MaximumTeamMembers], [MonthlyAICredits], [MonthlyPrice], [Name], [Plan], [PrioritySupport], [StorageLimit], [TemplateAccess], [UpdatedAt], [UpdatedBy], [WhiteLabel], [YearlyPrice])
    VALUES (''20000000-0000-0000-0000-000000000001'', CAST(0 AS bit), ''2026-07-28T00:00:00.0000000Z'', NULL, N''USD'', CAST(0 AS bit), CAST(0 AS bit), 1, CAST(1 AS bit), CAST(0 AS bit), 3, 1, 1, 10, 0.0, N''Free'', N''Free'', CAST(0 AS bit), CAST(1073741824 AS bigint), N''Standard'', NULL, NULL, CAST(0 AS bit), 0.0),
    (''20000000-0000-0000-0000-000000000002'', CAST(0 AS bit), ''2026-07-28T00:00:00.0000000Z'', NULL, N''USD'', CAST(1 AS bit), CAST(0 AS bit), 25, CAST(1 AS bit), CAST(0 AS bit), 25, 2, 3, 250, 19.0, N''Creator'', N''Creator'', CAST(0 AS bit), CAST(53687091200 AS bigint), N''Premium'', NULL, NULL, CAST(0 AS bit), 190.0),
    (''20000000-0000-0000-0000-000000000003'', CAST(1 AS bit), ''2026-07-28T00:00:00.0000000Z'', NULL, N''USD'', CAST(1 AS bit), CAST(0 AS bit), 1000, CAST(1 AS bit), CAST(0 AS bit), 200, 10, 25, 2500, 79.0, N''Business'', N''Business'', CAST(1 AS bit), CAST(536870912000 AS bigint), N''All'', NULL, NULL, CAST(0 AS bit), 790.0),
    (''20000000-0000-0000-0000-000000000004'', CAST(1 AS bit), ''2026-07-28T00:00:00.0000000Z'', NULL, N''USD'', CAST(1 AS bit), CAST(1 AS bit), 100000, CAST(1 AS bit), CAST(0 AS bit), 10000, 100, 1000, 25000, 299.0, N''Enterprise'', N''Enterprise'', CAST(1 AS bit), CAST(2199023255552 AS bigint), N''All'', NULL, NULL, CAST(1 AS bit), 2990.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ApiAccess', N'CreatedAt', N'CreatedBy', N'Currency', N'CustomBranding', N'CustomDomains', N'GalleryLimit', N'IsActive', N'IsDeleted', N'MaximumEvents', N'MaximumOrganizations', N'MaximumTeamMembers', N'MonthlyAICredits', N'MonthlyPrice', N'Name', N'Plan', N'PrioritySupport', N'StorageLimit', N'TemplateAccess', N'UpdatedAt', N'UpdatedBy', N'WhiteLabel', N'YearlyPrice') AND [object_id] = OBJECT_ID(N'[billing_plans]'))
        SET IDENTITY_INSERT [billing_plans] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_billing_coupons_Code_OrganizationId] ON [billing_coupons] ([Code], [OrganizationId]) WHERE [OrganizationId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE UNIQUE INDEX [IX_billing_plans_Plan] ON [billing_plans] ([Plan]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE UNIQUE INDEX [IX_billing_webhooks_Provider_ExternalId] ON [billing_webhooks] ([Provider], [ExternalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE UNIQUE INDEX [IX_invoices_InvoiceNumber] ON [invoices] ([InvoiceNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE INDEX [IX_invoices_OrganizationId_IssueDate] ON [invoices] ([OrganizationId], [IssueDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE INDEX [IX_payment_transactions_OrganizationId_CreatedAt] ON [payment_transactions] ([OrganizationId], [CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE UNIQUE INDEX [IX_payment_transactions_Provider_ProviderReference] ON [payment_transactions] ([Provider], [ProviderReference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    CREATE UNIQUE INDEX [IX_usage_counters_OrganizationId_PeriodStart_Metric] ON [usage_counters] ([OrganizationId], [PeriodStart], [Metric]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728155127_AddBillingSubscriptionsLicensing'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260728155127_AddBillingSubscriptionsLicensing', N'9.0.18');
END;

COMMIT;
GO

