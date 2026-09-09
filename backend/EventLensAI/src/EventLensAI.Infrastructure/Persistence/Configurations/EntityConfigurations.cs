using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventLensAI.Infrastructure.Persistence.Configurations;

internal static class ConfigurationExtensions
{
    public static void ConfigureBase<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : EventLensAI.Domain.Common.BaseEntity
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users"); b.ConfigureBase();
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Email).HasMaxLength(320).IsRequired();
        b.Property(x => x.NormalizedEmail).HasMaxLength(320).IsRequired();
        b.HasIndex(x => x.NormalizedEmail).IsUnique();
        b.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        b.Property(x => x.MustChangePassword).HasDefaultValue(false).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(30);
        b.Property(x => x.ProfileImage).HasMaxLength(2048);
        b.Ignore(x => x.TimeZone);
        b.Ignore(x => x.Language);
        b.Ignore(x => x.EmailVerified);
        b.Ignore(x => x.FailedLoginCount);
        b.Ignore(x => x.LockoutEnd);
        b.Ignore(x => x.IsLockedOut);
    }
}

internal sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> b)
    {
        b.ToTable("organizations"); b.ConfigureBase();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Slug).HasMaxLength(100).IsRequired();
        b.HasIndex(x => x.Slug).IsUnique();
        b.Property(x => x.Logo).HasMaxLength(2048);
        b.Property(x => x.Website).HasMaxLength(2048);
        b.Property(x => x.Description).HasMaxLength(4000);
        b.Property(x => x.Email).HasMaxLength(320);
        b.Property(x => x.Phone).HasMaxLength(30);
        b.Property(x => x.Address).HasMaxLength(1000);
        b.Property(x => x.Country).HasMaxLength(100);
        b.Property(x => x.TimeZone).HasMaxLength(100);
        b.Property(x => x.PrimaryColor).HasMaxLength(7);
        b.Property(x => x.SecondaryColor).HasMaxLength(7);
        b.Property(x => x.Plan).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.OrganizationType).HasConversion<string>().HasMaxLength(40);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
    }
}

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    private static readonly DateTime SeedDate = new(2026, 7, 28, 0, 0, 0, DateTimeKind.Utc);
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("roles"); b.ConfigureBase();
        b.Property(x => x.Name).HasMaxLength(50).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.HasData(
            Seed(SystemRoles.SuperAdminId, SystemRoles.SuperAdmin),
            Seed(SystemRoles.OwnerId, SystemRoles.Owner),
            Seed(SystemRoles.ManagerId, SystemRoles.Manager),
            Seed(SystemRoles.PhotographerId, SystemRoles.Photographer),
            Seed(SystemRoles.GuestId, SystemRoles.Guest),
            Seed(SystemRoles.EditorId, SystemRoles.Editor),
            Seed(SystemRoles.ViewerId, SystemRoles.Viewer),
            Seed(SystemRoles.BoothOperatorId, SystemRoles.BoothOperator),
            Seed(SystemRoles.DesignerId, SystemRoles.Designer),
            Seed(SystemRoles.MarketingManagerId, SystemRoles.MarketingManager),
            Seed(SystemRoles.PlatformAdminId, SystemRoles.PlatformAdmin));
    }
    private static object Seed(Guid id, string name) => new
    {
        Id = id, Name = name, CreatedAt = SeedDate, UpdatedAt = (DateTime?)null,
        CreatedBy = (Guid?)null, UpdatedBy = (Guid?)null, IsDeleted = false
    };
}

internal sealed class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> b)
    {
        b.ToTable("organization_members"); b.ConfigureBase();
        b.HasIndex(x => new { x.OrganizationId, x.UserId }).IsUnique();
        b.HasIndex(x => x.OrganizationId);
        b.HasIndex(x => x.UserId);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.InvitedBy);
        b.HasOne(x => x.Organization).WithMany(x => x.Members).HasForeignKey(x => x.OrganizationId);
        b.HasOne(x => x.User).WithMany(x => x.Memberships).HasForeignKey(x => x.UserId);
        b.HasOne(x => x.Role).WithMany(x => x.Members).HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> b)
    {
        b.ToTable("events"); b.ConfigureBase();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Slug).HasMaxLength(120).IsRequired();
        b.HasIndex(x => new { x.OrganizationId, x.Slug }).IsUnique();
        b.Property(x => x.Description).HasMaxLength(4000);
        b.Property(x => x.Venue).HasMaxLength(500);
        b.Property(x => x.Address).HasMaxLength(1000);
        b.Property(x => x.Logo).HasMaxLength(2048);
        b.Property(x => x.PrimaryColor).HasMaxLength(7);
        b.Property(x => x.SecondaryColor).HasMaxLength(7);
        b.Property(x => x.EventType).HasConversion<string>().HasMaxLength(40);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.PublicUrl).HasMaxLength(2048);
        b.Property(x => x.QRCodeUrl).HasMaxLength(2048);
        b.Property(x => x.QRCodeSvg).HasColumnType("nvarchar(max)");
        b.Property(x=>x.Timezone).HasMaxLength(100).IsRequired();
        b.Property(x=>x.City).HasMaxLength(120); b.Property(x=>x.Country).HasMaxLength(120);
        b.Property(x=>x.ContactPerson).HasMaxLength(200); b.Property(x=>x.ContactEmail).HasMaxLength(320);
        b.Property(x=>x.ContactPhone).HasMaxLength(50);
        b.HasIndex(x=>x.OrganizationId); b.HasIndex(x=>x.Status); b.HasIndex(x=>x.StartDate); b.HasIndex(x=>x.EventTypeId);
        b.HasOne(x => x.Organization).WithMany(x => x.Events).HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x=>x.Branch).WithMany().HasForeignKey(x=>x.BranchId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x=>x.EventTypeDefinition).WithMany(x=>x.Events).HasForeignKey(x=>x.EventTypeId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x=>x.AssignedManager).WithMany().HasForeignKey(x=>x.AssignedManagerId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x=>x.BrandProfile).WithMany().HasForeignKey(x=>x.BrandProfileId).OnDelete(DeleteBehavior.NoAction);
    }
}

internal sealed class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> b)
    {
        b.ToTable("photos"); b.ConfigureBase();
        b.Property(x => x.OriginalImageUrl).HasMaxLength(2048).IsRequired();
        b.Property(x => x.ProcessedImageUrl).HasMaxLength(2048);
        b.Property(x => x.ThumbnailUrl).HasMaxLength(2048);
        b.Property(x => x.Format).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.ProcessingType).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.FailureReason).HasMaxLength(1000);
        b.HasOne(x => x.Event).WithMany(x => x.Photos).HasForeignKey(x => x.EventId);
        // SQL Server rejects Event -> Photo and Event -> BoothSession -> Photo as
        // multiple cascade paths. Session deletion is handled explicitly by the
        // application; the database must not cascade or set this FK automatically.
        b.HasOne(x => x.Session).WithMany().HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.NoAction);
    }
}

internal sealed class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> b)
    {
        b.ToTable("templates"); b.ConfigureBase();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Category).HasConversion<string>().HasMaxLength(100).IsRequired();
        b.Property(x => x.PreviewImage).HasMaxLength(2048);
        b.Property(x => x.ConfigurationJson).HasColumnType("nvarchar(max)").IsRequired();
        b.HasOne(x => x.Organization).WithMany().HasForeignKey(x => x.OrganizationId);
        b.HasIndex(x => new { x.OrganizationId, x.Name });
    }
}

internal sealed class GalleryConfiguration : IEntityTypeConfiguration<Gallery>
{
    public void Configure(EntityTypeBuilder<Gallery> b)
    {
        b.ToTable("galleries"); b.ConfigureBase();
        b.Property(x => x.PublicUrl).HasMaxLength(2048).IsRequired();
        b.Property(x => x.QRCode).HasMaxLength(2048).IsRequired();
        b.HasIndex(x => x.PublicUrl).IsUnique();
        b.HasOne(x => x.Event).WithMany(x => x.Galleries).HasForeignKey(x => x.EventId);
    }
}

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> b)
    {
        b.ToTable("subscriptions"); b.ConfigureBase();
        b.Property(x => x.Plan).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.BillingInterval).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Provider).HasMaxLength(50);
        b.Property(x => x.ProviderSubscriptionId).HasMaxLength(200);
        b.HasOne(x => x.Organization).WithMany(x => x.Subscriptions).HasForeignKey(x => x.OrganizationId);
    }
}

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("refresh_tokens"); b.ConfigureBase();
        b.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => new { x.UserId, x.FamilyId });
        b.Property(x=>x.DeviceInfo).HasMaxLength(500); b.Property(x=>x.IpAddress).HasMaxLength(64);
        b.Ignore(x => x.IsActive);
        b.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>{public void Configure(EntityTypeBuilder<UserToken>b){b.ToTable("user_tokens");b.ConfigureBase();b.Property(x=>x.TokenHash).HasMaxLength(64).IsRequired();b.Property(x=>x.Purpose).HasMaxLength(30).IsRequired();b.HasIndex(x=>x.TokenHash).IsUnique();b.HasIndex(x=>new{x.UserId,x.Purpose});b.Ignore(x=>x.IsValid);b.HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);}}
internal sealed class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>{public void Configure(EntityTypeBuilder<UserPreference>b){b.ToTable("user_preferences");b.ConfigureBase();b.Property(x=>x.Theme).HasMaxLength(20);b.Property(x=>x.Language).HasMaxLength(10);b.HasIndex(x=>x.UserId).IsUnique();b.HasOne<User>().WithOne().HasForeignKey<UserPreference>(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);}}
internal sealed class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>{public void Configure(EntityTypeBuilder<ActivityLog>b){b.ToTable("activity_logs");b.ConfigureBase();b.Property(x=>x.Action).HasMaxLength(80).IsRequired();b.Property(x=>x.Description).HasMaxLength(500).IsRequired();b.Property(x=>x.IpAddress).HasMaxLength(64);b.Property(x=>x.Device).HasMaxLength(500);b.HasIndex(x=>new{x.UserId,x.CreatedAt});}}
internal sealed class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>{public void Configure(EntityTypeBuilder<ApiKey>b){b.ToTable("api_keys");b.ConfigureBase();b.Property(x=>x.Name).HasMaxLength(100).IsRequired();b.Property(x=>x.Prefix).HasMaxLength(20).IsRequired();b.Property(x=>x.KeyHash).HasMaxLength(64).IsRequired();b.HasIndex(x=>x.KeyHash).IsUnique();b.HasIndex(x=>x.UserId);b.Ignore(x=>x.IsActive);}}


internal sealed class OrganizationInvitationConfiguration : IEntityTypeConfiguration<OrganizationInvitation>
{
    public void Configure(EntityTypeBuilder<OrganizationInvitation> b)
    {
        b.ToTable("organization_invitations"); b.ConfigureBase();
        b.Property(x => x.Email).HasMaxLength(320).IsRequired();
        b.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => x.Email);
        b.HasIndex(x => new { x.OrganizationId, x.Email });
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.Ignore(x => x.IsValid);
        b.HasOne(x => x.Organization).WithMany(x => x.Invitations).HasForeignKey(x => x.OrganizationId);
        b.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
    }
}
internal sealed class EventSettingsConfiguration : IEntityTypeConfiguration<EventSettings>
{
    public void Configure(EntityTypeBuilder<EventSettings> b)
    {
        b.ToTable("event_settings"); b.ConfigureBase();
        b.HasIndex(x => x.EventId).IsUnique();
        b.Property(x => x.CaptureMode).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.Language).HasMaxLength(10);
        b.Property(x => x.Watermark).HasMaxLength(2048);
        b.Property(x => x.DefaultFilter).HasMaxLength(100);
        b.HasOne(x => x.Event).WithOne(x => x.Settings).HasForeignKey<EventSettings>(x => x.EventId);
    }
}
internal sealed class EventBrandingConfiguration : IEntityTypeConfiguration<EventBranding>
{
    public void Configure(EntityTypeBuilder<EventBranding> b)
    {
        b.ToTable("event_branding"); b.ConfigureBase();
        b.HasIndex(x => x.EventId).IsUnique();
        b.Property(x => x.Logo).HasMaxLength(2048); b.Property(x => x.Watermark).HasMaxLength(2048);
        b.Property(x => x.Background).HasMaxLength(2048); b.Property(x => x.SplashScreen).HasMaxLength(2048);
        b.Property(x => x.BrandFontsJson).HasColumnType("nvarchar(max)");
        b.Property(x => x.BrandColorsJson).HasColumnType("nvarchar(max)");
        b.Property(x => x.CustomCss).HasColumnType("nvarchar(max)");
        b.HasOne(x => x.Event).WithOne(x => x.Branding).HasForeignKey<EventBranding>(x => x.EventId);
    }
}
internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("audit_logs"); b.ConfigureBase();
        b.Property(x => x.ResourceType).HasMaxLength(100).IsRequired();
        b.Property(x => x.Action).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.IPAddress).HasMaxLength(64);
        b.HasIndex(x => new { x.ResourceType, x.ResourceId, x.CreatedAt });
    }
}
