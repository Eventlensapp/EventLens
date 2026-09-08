using EventLensAI.Application.Common;
using EventLensAI.Application.Features.Billing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Route("api/billing"),Produces("application/json")]
public sealed class BillingController(IBillingService service):ControllerBase
{
 /// <summary>Lists the active public SaaS plan catalog and plan entitlements.</summary>
 [AllowAnonymous,HttpGet("plans")]public async Task<ActionResult<ApiResponse<IReadOnlyList<PlanDto>>>>Plans(CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<PlanDto>>.Ok(await service.PlansAsync(ct)));
 [Authorize(Policy="BillingOwner"),HttpGet("organizations/{organizationId:guid}/subscription")]public async Task<ActionResult<ApiResponse<SubscriptionDto>>>Subscription(Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<SubscriptionDto>.Ok(await service.SubscriptionAsync(organizationId,ct)));
 [Authorize(Policy="BillingOwner"),HttpPost("organizations/{organizationId:guid}/subscription")]public async Task<ActionResult<ApiResponse<CheckoutDto>>>Change(Guid organizationId,ChangeSubscriptionRequest request,CancellationToken ct)=>Ok(ApiResponse<CheckoutDto>.Ok(await service.ChangeSubscriptionAsync(organizationId,request,ct),"Checkout created."));
 [Authorize(Policy="BillingOwner"),HttpPost("organizations/{organizationId:guid}/subscription/cancel")]public async Task<ActionResult<ApiResponse<SubscriptionDto>>>Cancel(Guid organizationId,CancelSubscriptionRequest request,CancellationToken ct)=>Ok(ApiResponse<SubscriptionDto>.Ok(await service.CancelAsync(organizationId,request.AtPeriodEnd,ct)));
 [Authorize(Policy="BillingOwner"),HttpPost("organizations/{organizationId:guid}/subscription/pause")]public async Task<ActionResult<ApiResponse<SubscriptionDto>>>Pause(Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<SubscriptionDto>.Ok(await service.PauseAsync(organizationId,ct)));
 [Authorize(Policy="BillingOwner"),HttpPost("organizations/{organizationId:guid}/subscription/resume")]public async Task<ActionResult<ApiResponse<SubscriptionDto>>>Resume(Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<SubscriptionDto>.Ok(await service.ResumeAsync(organizationId,ct)));
 [Authorize(Policy="BillingOwner"),HttpGet("organizations/{organizationId:guid}/usage")]public async Task<ActionResult<ApiResponse<UsageDto>>>Usage(Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<UsageDto>.Ok(await service.UsageAsync(organizationId,ct)));
 [Authorize(Policy="BillingOwner"),HttpGet("organizations/{organizationId:guid}/invoices")]public async Task<ActionResult<ApiResponse<PagedResult<InvoiceDto>>>>Invoices(Guid organizationId,[FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)=>Ok(ApiResponse<PagedResult<InvoiceDto>>.Ok(await service.InvoicesAsync(organizationId,page,pageSize,ct)));
 [Authorize(Policy="BillingOwner"),HttpGet("invoices/{id:guid}")]public async Task<ActionResult<ApiResponse<InvoiceDto>>>Invoice(Guid id,CancellationToken ct)=>Ok(ApiResponse<InvoiceDto>.Ok(await service.InvoiceAsync(id,ct)));
 [Authorize(Policy="BillingOwner"),HttpGet("organizations/{organizationId:guid}/payments")]public async Task<ActionResult<ApiResponse<PagedResult<PaymentDto>>>>Payments(Guid organizationId,[FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)=>Ok(ApiResponse<PagedResult<PaymentDto>>.Ok(await service.PaymentsAsync(organizationId,page,pageSize,ct)));
 [Authorize(Policy="BillingOwner"),HttpGet("organizations/{organizationId:guid}/payment-methods")]public async Task<ActionResult<ApiResponse<IReadOnlyList<string>>>>Methods(Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<string>>.Ok(await service.PaymentMethodsAsync(organizationId,ct)));
 /// <summary>Receives signed, idempotent payment-provider events.</summary>
 [AllowAnonymous,HttpPost("webhooks/{provider}")]public async Task<ActionResult<ApiResponse<object>>>Webhook(string provider,WebhookRequest request,CancellationToken ct){await service.HandleWebhookAsync(provider,request,ct);return Ok(ApiResponse<object>.Ok(new{},"Webhook processed."));}
}
