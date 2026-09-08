using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventLensAI.Infrastructure.Persistence.Configurations;

internal sealed class BoothSessionConfiguration : IEntityTypeConfiguration<BoothSession>
{
    public void Configure(EntityTypeBuilder<BoothSession> b)
    {
        b.ToTable("booth_sessions"); b.ConfigureBase(); b.Ignore(x => x.SessionId);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.CaptureMode).HasConversion<string>().HasMaxLength(30);
        b.Property(x=>x.SessionType).HasConversion<string>().HasMaxLength(20);
        b.Property(x=>x.SessionToken).HasMaxLength(100);
        b.Property(x=>x.DeviceInformation).HasMaxLength(1000);
        b.Property(x=>x.GuestName).HasMaxLength(120);
        b.Property(x=>x.GuestEmail).HasMaxLength(320);
        b.Property(x=>x.MetadataJson).HasMaxLength(4000);
        b.HasIndex(x => new { x.EventId, x.StartedAt });
        b.HasIndex(x=>x.OrganizationId);b.HasIndex(x=>x.SessionToken).IsUnique().HasFilter("[SessionToken] IS NOT NULL");b.HasIndex(x=>x.Status);b.HasIndex(x=>x.StartedAt);
        b.HasOne<Organization>().WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Event).WithMany().HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class BoothSessionActivityConfiguration:IEntityTypeConfiguration<BoothSessionActivity>
{
    public void Configure(EntityTypeBuilder<BoothSessionActivity>b)
    {
        b.ToTable("booth_session_activities");b.ConfigureBase();b.Property(x=>x.Action).HasMaxLength(40).IsRequired();b.Property(x=>x.Metadata).HasMaxLength(4000);
        b.HasIndex(x=>new{x.SessionId,x.Timestamp});b.HasOne(x=>x.Session).WithMany().HasForeignKey(x=>x.SessionId).OnDelete(DeleteBehavior.Restrict);
    }
}
