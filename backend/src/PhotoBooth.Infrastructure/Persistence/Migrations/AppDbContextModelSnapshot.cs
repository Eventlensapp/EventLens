using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace PhotoBooth.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
public partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

        modelBuilder.Entity("PhotoBooth.Domain.Entities.Event", entity =>
        {
            entity.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            entity.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<string>("Name").IsRequired().HasMaxLength(200);
            entity.Property<DateTime>("StartsAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<DateTime>("EndsAtUtc").HasColumnType("timestamp with time zone");
            entity.HasKey("Id");
            entity.HasIndex("StartsAtUtc");
            entity.ToTable("events");
        });
        modelBuilder.Entity("PhotoBooth.Domain.Entities.Template", entity =>
        {
            entity.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            entity.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<string>("Name").IsRequired().HasMaxLength(200);
            entity.Property<string>("Definition").IsRequired().HasColumnType("jsonb");
            entity.HasKey("Id");
            entity.ToTable("templates");
        });
        modelBuilder.Entity("PhotoBooth.Domain.Entities.User", entity =>
        {
            entity.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            entity.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<string>("Email").IsRequired().HasMaxLength(320);
            entity.Property<string>("NormalizedEmail").IsRequired().HasMaxLength(320);
            entity.Property<string>("PasswordHash").IsRequired().HasMaxLength(512);
            entity.Property<string>("DisplayName").IsRequired().HasMaxLength(100);
            entity.Property<string>("Role").IsRequired().HasMaxLength(50);
            entity.Property<bool>("IsEmailVerified");
            entity.Property<bool>("IsActive");
            entity.Property<DateTime?>("LastLoginAtUtc").HasColumnType("timestamp with time zone");
            entity.HasKey("Id");
            entity.HasIndex("NormalizedEmail").IsUnique();
            entity.ToTable("users");
        });
        modelBuilder.Entity("PhotoBooth.Domain.Entities.Photo", entity =>
        {
            entity.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            entity.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<Guid>("EventId").HasColumnType("uuid");
            entity.Property<Guid>("TemplateId").HasColumnType("uuid");
            entity.Property<string>("StorageUrl").IsRequired().HasMaxLength(2048);
            entity.HasKey("Id");
            entity.HasIndex("EventId");
            entity.HasIndex("TemplateId");
            entity.ToTable("photos");
        });
        modelBuilder.Entity("PhotoBooth.Domain.Entities.RefreshToken", entity =>
        {
            entity.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");
            entity.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<Guid>("UserId").HasColumnType("uuid");
            entity.Property<string>("TokenHash").IsRequired().HasMaxLength(64);
            entity.Property<Guid>("FamilyId").HasColumnType("uuid");
            entity.Property<DateTime>("ExpiresAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<DateTime?>("RevokedAtUtc").HasColumnType("timestamp with time zone");
            entity.Property<Guid?>("ReplacedByTokenId").HasColumnType("uuid");
            entity.HasKey("Id");
            entity.HasIndex("TokenHash").IsUnique();
            entity.HasIndex("UserId", "FamilyId");
            entity.ToTable("refresh_tokens");
        });
        modelBuilder.Entity("PhotoBooth.Domain.Entities.Photo", entity =>
        {
            entity.HasOne("PhotoBooth.Domain.Entities.Event", "Event").WithMany("Photos")
                .HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            entity.HasOne("PhotoBooth.Domain.Entities.Template", "Template").WithMany("Photos")
                .HasForeignKey("TemplateId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        });
        modelBuilder.Entity("PhotoBooth.Domain.Entities.RefreshToken", entity =>
        {
            entity.HasOne("PhotoBooth.Domain.Entities.User", "User").WithMany("RefreshTokens")
                .HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        });
    }
}
