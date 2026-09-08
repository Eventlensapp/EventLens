using System.Data;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Features.Billing;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace EventLensAI.Infrastructure.Services;
public sealed class FeaturePermissionService(EventLensDbContext db):IFeaturePermissionService
{
 static DateOnly Period=>new(DateTime.UtcNow.Year,DateTime.UtcNow.Month,1);
 async Task<(Organization org,PlanDefinition plan)>Context(Guid id,CancellationToken ct){var org=await db.Organizations.SingleOrDefaultAsync(x=>x.Id==id,ct)??throw new NotFoundException("Organization not found.");if(!org.IsActive)throw new ConflictException("Organization is suspended.");var plan=await db.PlanDefinitions.AsNoTracking().SingleAsync(x=>x.Plan==org.Plan&&x.IsActive,ct);return(org,plan);}
 async Task<long>Used(Guid org,UsageMetric metric,CancellationToken ct)=>await db.UsageCounters.Where(x=>x.OrganizationId==org&&x.PeriodStart==Period&&x.Metric==metric).Select(x=>x.Quantity).SingleOrDefaultAsync(ct);
 public async Task<bool>CanUseAIAsync(Guid id,int credits,CancellationToken ct){var(_,p)=await Context(id,ct);return await Used(id,UsageMetric.AIGenerations,ct)+credits<=p.MonthlyAICredits;}
 public async Task<bool>CanUploadLogoAsync(Guid id,CancellationToken ct)=>(await Context(id,ct)).plan.CustomBranding;
 public async Task<bool>CanCreateEventAsync(Guid id,CancellationToken ct){var(_,p)=await Context(id,ct);return await db.Events.CountAsync(x=>x.OrganizationId==id,ct)<p.MaximumEvents;}
 public async Task<bool>CanUseCustomTemplatesAsync(Guid id,CancellationToken ct)=>(await Context(id,ct)).plan.TemplateAccess!="Standard";
 public async Task<bool>CanExportPDFAsync(Guid id,CancellationToken ct)=>(await Context(id,ct)).plan.Plan>=SubscriptionPlan.Business;
 public async Task<bool>CanUseWhiteLabelAsync(Guid id,CancellationToken ct)=>(await Context(id,ct)).plan.WhiteLabel;
 public async Task<bool>CanCreateCustomDomainAsync(Guid id,CancellationToken ct)=>(await Context(id,ct)).plan.CustomDomains;
 public async Task EnsureAsync(Guid id,BillingFeature feature,long amount,CancellationToken ct){var(org,p)=await Context(id,ct);var allowed=feature switch{BillingFeature.Events=>await db.Events.CountAsync(x=>x.OrganizationId==id,ct)+amount<=p.MaximumEvents,BillingFeature.TeamMembers=>await db.OrganizationMembers.CountAsync(x=>x.OrganizationId==id,ct)+amount<=p.MaximumTeamMembers,BillingFeature.AICredits=>await Used(id,UsageMetric.AIGenerations,ct)+amount<=p.MonthlyAICredits,BillingFeature.Storage=>org.StorageUsed+amount<=p.StorageLimit,BillingFeature.Galleries=>await db.Galleries.CountAsync(x=>x.Event.OrganizationId==id,ct)+amount<=p.GalleryLimit,BillingFeature.CustomTemplates=>p.TemplateAccess!="Standard",BillingFeature.CustomBranding=>p.CustomBranding,BillingFeature.WhiteLabel=>p.WhiteLabel,BillingFeature.ApiAccess=>p.ApiAccess,BillingFeature.CustomDomains=>p.CustomDomains,BillingFeature.PrioritySupport=>p.PrioritySupport,BillingFeature.PdfExport=>p.Plan>=SubscriptionPlan.Business,_=>true};if(!allowed)throw new ConflictException($"{feature} is unavailable or its plan limit has been reached.");}
 public async Task TrackAsync(Guid id,UsageMetric metric,long quantity,CancellationToken ct)
 {
  if(quantity==0)return;
  var strategy=db.Database.CreateExecutionStrategy();
  await strategy.ExecuteAsync(async()=>
  {
   await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
   var row=await db.UsageCounters.SingleOrDefaultAsync(x=>x.OrganizationId==id&&x.PeriodStart==Period&&x.Metric==metric,ct);
   if(row is null)db.Add(new UsageCounter(id,metric,Period,Math.Max(0,quantity)));else row.Add(quantity);
   if(metric==UsageMetric.StorageConsumed)(await db.Organizations.SingleAsync(x=>x.Id==id,ct)).AddStorage(quantity);
   await db.SaveChangesAsync(ct);
   await tx.CommitAsync(ct);
  });
 }
}
