using EventLensAI.Application.Common;
using EventLensAI.Domain.Enums;
namespace EventLensAI.Application.Features.Billing;
public sealed record PlanDto(SubscriptionPlan Plan,string Name,decimal MonthlyPrice,decimal YearlyPrice,string Currency,int MaximumOrganizations,int MaximumEvents,int MaximumTeamMembers,int MonthlyAICredits,long StorageLimit,int GalleryLimit,string TemplateAccess,bool CustomBranding,bool WhiteLabel,bool ApiAccess,bool CustomDomains,bool PrioritySupport);
public sealed record SubscriptionDto(Guid Id,Guid OrganizationId,SubscriptionPlan Plan,SubscriptionStatus Status,BillingInterval BillingInterval,DateTime StartDate,DateTime EndDate,DateTime? TrialEndsAt,DateTime? GracePeriodEndsAt,bool CancelAtPeriodEnd,string? Provider);
public sealed record ChangeSubscriptionRequest(SubscriptionPlan Plan,BillingInterval Interval,string? Provider,string? CouponCode,bool StartTrial=false);
public sealed record CancelSubscriptionRequest(bool AtPeriodEnd=true);
public sealed record InvoiceDto(Guid Id,string InvoiceNumber,string Customer,SubscriptionPlan Plan,decimal Amount,decimal Tax,decimal Discount,string Currency,InvoiceStatus Status,DateTime IssueDate,DateTime DueDate);
public sealed record PaymentDto(Guid Id,string Provider,string ProviderReference,decimal Amount,string Currency,PaymentStatus Status,decimal RefundedAmount,DateTime CreatedAt);
public sealed record UsageDto(Guid OrganizationId,DateOnly PeriodStart,IReadOnlyDictionary<UsageMetric,long> Usage,IReadOnlyDictionary<UsageMetric,long> Limits);
public sealed record CheckoutDto(string Provider,string Reference,string? CheckoutUrl);
public sealed record CouponRequest(string Code,DiscountType DiscountType,decimal Value,DateTime Expiry,int UsageLimit,Guid? OrganizationId,bool IsReferral);
public sealed record RefundRequest(decimal Amount,string Reason);
public sealed record GrantCreditsRequest(long Credits,string Reason);
public sealed record WebhookRequest(string ExternalId,BillingWebhookType Type,string Payload,string Signature);
public sealed record PaymentCheckoutRequest(Guid OrganizationId,SubscriptionPlan Plan,BillingInterval Interval,decimal Amount,string Currency,string CustomerEmail,string? CouponCode);
public sealed record PaymentCheckoutResult(string Reference,string CheckoutUrl);
public sealed record PaymentWebhookResult(string ExternalId,BillingWebhookType Type,string ProviderReference,Guid OrganizationId,SubscriptionPlan Plan,BillingInterval Interval,decimal Amount,string Currency);
public interface IPaymentProvider{string Name{get;}Task<PaymentCheckoutResult>CreateCheckoutAsync(PaymentCheckoutRequest request,CancellationToken ct);Task CancelSubscriptionAsync(string providerSubscriptionId,bool atPeriodEnd,CancellationToken ct);Task PauseSubscriptionAsync(string providerSubscriptionId,CancellationToken ct);Task ResumeSubscriptionAsync(string providerSubscriptionId,CancellationToken ct);Task<string>RefundAsync(string providerReference,decimal amount,string reason,CancellationToken ct);Task<PaymentWebhookResult>ParseWebhookAsync(string payload,string signature,CancellationToken ct);}
public interface IPaymentProviderResolver{IReadOnlyList<string>AvailableProviders{get;}IPaymentProvider Resolve(string name);}
public interface IFeaturePermissionService
{
 Task<bool>CanUseAIAsync(Guid organizationId,int credits,CancellationToken ct);Task<bool>CanUploadLogoAsync(Guid organizationId,CancellationToken ct);Task<bool>CanCreateEventAsync(Guid organizationId,CancellationToken ct);Task<bool>CanUseCustomTemplatesAsync(Guid organizationId,CancellationToken ct);Task<bool>CanExportPDFAsync(Guid organizationId,CancellationToken ct);Task<bool>CanUseWhiteLabelAsync(Guid organizationId,CancellationToken ct);Task<bool>CanCreateCustomDomainAsync(Guid organizationId,CancellationToken ct);Task EnsureAsync(Guid organizationId,BillingFeature feature,long amount,CancellationToken ct);Task TrackAsync(Guid organizationId,UsageMetric metric,long quantity,CancellationToken ct);
}
public interface IBillingService
{
 Task<IReadOnlyList<PlanDto>>PlansAsync(CancellationToken ct);Task<SubscriptionDto>SubscriptionAsync(Guid organizationId,CancellationToken ct);Task<CheckoutDto>ChangeSubscriptionAsync(Guid organizationId,ChangeSubscriptionRequest request,CancellationToken ct);Task<SubscriptionDto>CancelAsync(Guid organizationId,bool atPeriodEnd,CancellationToken ct);Task<SubscriptionDto>PauseAsync(Guid organizationId,CancellationToken ct);Task<SubscriptionDto>ResumeAsync(Guid organizationId,CancellationToken ct);Task<UsageDto>UsageAsync(Guid organizationId,CancellationToken ct);Task<PagedResult<InvoiceDto>>InvoicesAsync(Guid organizationId,int page,int pageSize,CancellationToken ct);Task<InvoiceDto>InvoiceAsync(Guid id,CancellationToken ct);Task<PagedResult<PaymentDto>>PaymentsAsync(Guid organizationId,int page,int pageSize,CancellationToken ct);Task HandleWebhookAsync(string provider,WebhookRequest request,CancellationToken ct);Task<IReadOnlyList<string>>PaymentMethodsAsync(Guid organizationId,CancellationToken ct);Task<PagedResult<SubscriptionDto>>AdminSubscriptionsAsync(int page,int pageSize,CancellationToken ct);Task RefundAsync(Guid paymentId,RefundRequest request,CancellationToken ct);Task GrantCreditsAsync(Guid organizationId,GrantCreditsRequest request,CancellationToken ct);Task SuspendAsync(Guid organizationId,bool suspended,CancellationToken ct);Task CreateCouponAsync(CouponRequest request,CancellationToken ct);
}
