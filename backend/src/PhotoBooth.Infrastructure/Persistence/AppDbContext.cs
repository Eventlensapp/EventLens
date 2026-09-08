using Microsoft.EntityFrameworkCore;
using PhotoBooth.Application.Abstractions.Persistence;
using PhotoBooth.Domain.Entities;

namespace PhotoBooth.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(builder =>
        {
            builder.ToTable("events");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.HasIndex(x => x.StartsAtUtc);
        });
        modelBuilder.Entity<Template>(builder =>
        {
            builder.ToTable("templates");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Definition).HasColumnType("jsonb").IsRequired();
        });
        modelBuilder.Entity<Photo>(builder =>
        {
            builder.ToTable("photos");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.StorageUrl).HasMaxLength(2048).IsRequired();
            builder.HasOne(x => x.Event).WithMany(x => x.Photos).HasForeignKey(x => x.EventId);
            builder.HasOne(x => x.Template).WithMany(x => x.Photos).HasForeignKey(x => x.TemplateId);
        });
        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).HasMaxLength(320).IsRequired();
            builder.Property(x => x.NormalizedEmail).HasMaxLength(320).IsRequired();
            builder.HasIndex(x => x.NormalizedEmail).IsUnique();
            builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            builder.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Role).HasMaxLength(50).IsRequired();
        });
        modelBuilder.Entity<RefreshToken>(builder =>
        {
            builder.ToTable("refresh_tokens");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.HasIndex(x => new { x.UserId, x.FamilyId });
            builder.Ignore(x => x.IsActive);
            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
