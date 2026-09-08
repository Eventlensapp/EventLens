using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventLensAI.Infrastructure.Persistence.Configurations;

internal sealed class BoothConfigurationConfiguration:IEntityTypeConfiguration<BoothConfiguration>
{
    public void Configure(EntityTypeBuilder<BoothConfiguration>b){b.ToTable("booth_configurations");b.ConfigureBase();b.Property(x=>x.BoothName).HasMaxLength(120).IsRequired();b.Property(x=>x.BoothMode).HasMaxLength(40);b.Property(x=>x.DefaultCamera).HasMaxLength(200);b.Property(x=>x.DefaultResolution).HasMaxLength(20);b.Property(x=>x.DefaultAspectRatio).HasMaxLength(20);b.Property(x=>x.Language).HasMaxLength(20);b.Property(x=>x.Theme).HasMaxLength(40);b.HasIndex(x=>x.OrganizationId).IsUnique().HasFilter("[IsDeleted] = 0");b.HasOne<Organization>().WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.Restrict);}
}

internal sealed class CameraPreferenceConfiguration:IEntityTypeConfiguration<CameraPreference>
{
    public void Configure(EntityTypeBuilder<CameraPreference>b)
    {
        b.ToTable("camera_preferences");b.ConfigureBase();
        b.Property(x=>x.PreferredCameraId).HasMaxLength(512);
        b.Property(x=>x.PreferredResolution).HasMaxLength(20).IsRequired();
        b.Property(x=>x.AspectRatio).HasMaxLength(20).IsRequired();
        b.HasIndex(x=>x.OrganizationId).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasOne<Organization>().WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
internal sealed class BoothCapabilityConfiguration:IEntityTypeConfiguration<BoothCapability>
{
    public void Configure(EntityTypeBuilder<BoothCapability>b){b.ToTable("booth_capabilities");b.ConfigureBase();b.Property(x=>x.Name).HasMaxLength(80);b.Property(x=>x.Source).HasMaxLength(40);b.HasIndex(x=>new{x.OrganizationId,x.Name});b.HasOne<Organization>().WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.Restrict);}
}
internal sealed class BoothDeviceConfiguration:IEntityTypeConfiguration<BoothDevice>
{
    public void Configure(EntityTypeBuilder<BoothDevice>b){b.ToTable("booth_devices");b.ConfigureBase();b.Property(x=>x.DeviceKeyHash).HasMaxLength(128);b.Property(x=>x.Kind).HasMaxLength(40);b.Property(x=>x.Label).HasMaxLength(160);b.Property(x=>x.MetadataJson).HasColumnType("nvarchar(max)");b.HasIndex(x=>new{x.OrganizationId,x.DeviceKeyHash}).IsUnique();b.HasIndex(x=>new{x.OrganizationId,x.Kind});b.HasOne<Organization>().WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.Restrict);}
}
internal sealed class BoothHealthCheckConfiguration:IEntityTypeConfiguration<BoothHealthCheck>
{
    public void Configure(EntityTypeBuilder<BoothHealthCheck>b){b.ToTable("booth_health_checks");b.ConfigureBase();b.Property(x=>x.Category).HasMaxLength(80);b.Property(x=>x.Status).HasMaxLength(20);b.Property(x=>x.Message).HasMaxLength(500);b.HasIndex(x=>new{x.OrganizationId,x.CheckedAt});b.HasOne<Organization>().WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.Restrict);}
}
