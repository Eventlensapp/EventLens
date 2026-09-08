using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EventLensAI.Infrastructure.Persistence.Configurations;
internal sealed class AIJobConfiguration:IEntityTypeConfiguration<AIJob>
{
 public void Configure(EntityTypeBuilder<AIJob>b){b.ToTable("ai_jobs");b.ConfigureBase();b.Property(x=>x.JobType).HasConversion<string>().HasMaxLength(50);b.Property(x=>x.Status).HasConversion<string>().HasMaxLength(30);b.Property(x=>x.Provider).HasMaxLength(100);b.Property(x=>x.Prompt).HasMaxLength(4000);b.Property(x=>x.NegativePrompt).HasMaxLength(2000);b.Property(x=>x.InputImage).HasMaxLength(2048);b.Property(x=>x.OutputImage).HasMaxLength(2048);b.Property(x=>x.ErrorMessage).HasMaxLength(2000);b.HasIndex(x=>new{x.EventId,x.Status,x.CreatedAt});b.HasOne(x=>x.Event).WithMany().HasForeignKey(x=>x.EventId);}
}
internal sealed class AIPromptConfiguration:IEntityTypeConfiguration<AIPromptDefinition>
{
 public void Configure(EntityTypeBuilder<AIPromptDefinition>b){b.ToTable("ai_prompt_definitions");b.ConfigureBase();b.Property(x=>x.Key).HasMaxLength(100);b.Property(x=>x.JobType).HasConversion<string>().HasMaxLength(50);b.Property(x=>x.Prompt).HasMaxLength(4000);b.Property(x=>x.NegativePrompt).HasMaxLength(2000);b.HasIndex(x=>new{x.Key,x.JobType}).IsUnique();}
}
internal sealed class AIBackgroundConfiguration:IEntityTypeConfiguration<AIBackground>
{
 public void Configure(EntityTypeBuilder<AIBackground>b){b.ToTable("ai_backgrounds");b.ConfigureBase();b.Property(x=>x.Name).HasMaxLength(200);b.Property(x=>x.Category).HasConversion<string>().HasMaxLength(50);b.Property(x=>x.ImageUrl).HasMaxLength(2048);b.HasIndex(x=>new{x.OrganizationId,x.Category});b.HasOne(x=>x.Organization).WithMany().HasForeignKey(x=>x.OrganizationId);}
}
