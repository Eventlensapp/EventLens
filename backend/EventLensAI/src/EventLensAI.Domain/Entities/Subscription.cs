using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class Subscription : BaseEntity
{
    private Subscription() { }
    public Subscription(Guid organizationId, SubscriptionPlan plan, DateTime startDate, DateTime endDate)
    {
        OrganizationId = organizationId;
        Plan = plan;
        StartDate = startDate.ToUniversalTime();
        EndDate = endDate.ToUniversalTime();
        Status = SubscriptionStatus.Active;
    }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public SubscriptionPlan Plan { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public BillingInterval BillingInterval { get; private set; } = BillingInterval.Monthly;
    public DateTime? TrialEndsAt { get; private set; }
    public DateTime? GracePeriodEndsAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public bool CancelAtPeriodEnd { get; private set; }
    public string? Provider { get; private set; }
    public string? ProviderSubscriptionId { get; private set; }
    public void ChangePlan(SubscriptionPlan plan)=>Plan=plan;
    public void SetBillingInterval(BillingInterval interval)=>BillingInterval=interval;
    public void Cancel(bool atPeriodEnd){CancelAtPeriodEnd=atPeriodEnd;CancelledAt=DateTime.UtcNow;if(!atPeriodEnd)Status=SubscriptionStatus.Canceled;}
    public void Pause()=>Status=SubscriptionStatus.Paused;
    public void Resume()=>Status=SubscriptionStatus.Active;
    public void Renew(DateTime end){EndDate=end.ToUniversalTime();BillingInterval=EndDate-DateTime.UtcNow>TimeSpan.FromDays(180)?BillingInterval.Yearly:BillingInterval.Monthly;Status=SubscriptionStatus.Active;CancelAtPeriodEnd=false;}
    public void StartTrial(DateTime endsAt){TrialEndsAt=endsAt.ToUniversalTime();Status=SubscriptionStatus.Trialing;}
    public void StartGracePeriod(DateTime endsAt){GracePeriodEndsAt=endsAt.ToUniversalTime();Status=SubscriptionStatus.GracePeriod;}
    public void AttachProvider(string provider,string providerSubscriptionId){Provider=provider;ProviderSubscriptionId=providerSubscriptionId;}
}
