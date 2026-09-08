using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventLensAI.Infrastructure.Persistence.Configurations;

internal sealed class PhotoTemplateConfiguration : IEntityTypeConfiguration<PhotoTemplate>
{
    public void Configure(EntityTypeBuilder<PhotoTemplate> b)
    {
        b.ToTable("PhotoTemplates"); b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000);
        b.Property(x => x.AspectRatio).HasMaxLength(20);
        b.HasIndex(x => new { x.OrganizationId, x.IsActive });
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
internal sealed class TemplateLayoutConfiguration : IEntityTypeConfiguration<TemplateLayout>
{
    public void Configure(EntityTypeBuilder<TemplateLayout> b)
    {
        b.ToTable("TemplateLayouts"); b.HasKey(x => x.Id);
        b.Property(x => x.BackgroundColor).HasMaxLength(20);
        b.Property(x => x.BackgroundImage).HasMaxLength(500);
        b.HasIndex(x => x.PhotoTemplateId).IsUnique();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
internal sealed class TemplateElementConfiguration : IEntityTypeConfiguration<TemplateElement>
{
    public void Configure(EntityTypeBuilder<TemplateElement> b)
    {
        b.ToTable("TemplateElements"); b.HasKey(x => x.Id);
        b.Property(x => x.StyleConfiguration).HasColumnType("nvarchar(max)");
        b.HasIndex(x => new { x.PhotoTemplateId, x.LayerOrder });
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
internal sealed class StickerConfiguration : IEntityTypeConfiguration<Sticker>
{
    public void Configure(EntityTypeBuilder<Sticker> b)
    {
        b.ToTable("Stickers"); b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(150); b.Property(x => x.ImagePath).HasMaxLength(500);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
internal sealed class TemplateAssignmentConfiguration : IEntityTypeConfiguration<TemplateAssignment>
{
    public void Configure(EntityTypeBuilder<TemplateAssignment> b)
    {
        b.ToTable("TemplateAssignments"); b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.OrganizationId, x.EventId }).IsUnique();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
