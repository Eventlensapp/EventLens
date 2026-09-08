using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventLensAI.Infrastructure.Persistence.Configurations;

internal sealed class EventTypeDefinitionConfiguration : IEntityTypeConfiguration<EventTypeDefinition>
{
    private static readonly string[] Names =
    ["Wedding","Birthday","Corporate","Graduation","School","Exhibition","Festival","Brand Promotion","Product Launch","Conference","Other"];

    public void Configure(EntityTypeBuilder<EventTypeDefinition> b)
    {
        b.ToTable("event_types"); b.ConfigureBase();
        b.Property(x=>x.Name).HasMaxLength(120).IsRequired();
        b.Property(x=>x.Description).HasMaxLength(1000);
        b.Property(x=>x.Icon).HasMaxLength(100);
        b.Property(x=>x.Color).HasMaxLength(7).IsRequired();
        b.Property(x=>x.Status).HasConversion<string>().HasMaxLength(30);
        b.HasIndex(x=>new{x.OrganizationId,x.Name}).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasOne(x=>x.Organization).WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.NoAction);
        b.HasData(Names.Select((name,index)=>new
        {
            Id=Guid.Parse($"10000000-0000-0000-0000-{index+1:000000000000}"), Name=name,
            Description=(string?)null, Icon=(string?)null, Color="#6C5CE7", IsSystemType=true, IsActive=true, OrganizationId=(Guid?)null,
            Status=Domain.Enums.EventTypeStatus.Active, CreatedAt=new DateTime(2026,1,1,0,0,0,DateTimeKind.Utc),
            UpdatedAt=(DateTime?)null,CreatedBy=(Guid?)null,UpdatedBy=(Guid?)null,IsDeleted=false
        }));
    }
}

internal sealed class EventTemplateConfiguration:IEntityTypeConfiguration<EventTemplate>
{
    public void Configure(EntityTypeBuilder<EventTemplate>b)
    {
        b.ToTable("event_templates");b.ConfigureBase();b.Property(x=>x.Name).HasMaxLength(200).IsRequired();
        b.Property(x=>x.Description).HasMaxLength(2000);b.Property(x=>x.ConfigurationJson).HasColumnType("nvarchar(max)");
        b.Property(x=>x.DefaultDuration).HasConversion(v=>(int)v.TotalMinutes,v=>TimeSpan.FromMinutes(v)).HasColumnName("DefaultDurationMinutes");
        b.HasIndex(x=>x.OrganizationId);b.HasIndex(x=>x.EventTypeId);
        b.HasIndex(x=>new{x.OrganizationId,x.Name}).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasOne(x=>x.Organization).WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x=>x.EventType).WithMany().HasForeignKey(x=>x.EventTypeId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x=>x.DefaultBrandProfile).WithMany().HasForeignKey(x=>x.DefaultBrandProfileId).OnDelete(DeleteBehavior.NoAction);
    }
}
internal sealed class TemplateUsageConfiguration:IEntityTypeConfiguration<TemplateUsage>
{
    public void Configure(EntityTypeBuilder<TemplateUsage>b)
    {
        b.ToTable("event_template_usages");b.ConfigureBase();b.HasIndex(x=>x.TemplateId);b.HasIndex(x=>x.EventId);
        b.HasOne(x=>x.Template).WithMany(x=>x.Usages).HasForeignKey(x=>x.TemplateId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x=>x.Event).WithMany().HasForeignKey(x=>x.EventId).OnDelete(DeleteBehavior.NoAction);
    }
}

internal sealed class EventMemberConfiguration : IEntityTypeConfiguration<EventMember>
{
    public void Configure(EntityTypeBuilder<EventMember> b)
    {
        b.ToTable("event_members"); b.ConfigureBase();
        b.Property(x=>x.Role).HasConversion<string>().HasMaxLength(40);
        b.HasIndex(x=>x.EventId); b.HasIndex(x=>x.UserId);
        b.HasIndex(x=>new{x.EventId,x.UserId}).IsUnique();
        b.HasOne(x=>x.Event).WithMany(x=>x.Members).HasForeignKey(x=>x.EventId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.NoAction);
    }
}
