using EventLensAI.Application.Common;
using EventLensAI.Application.Features.Billing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize(Policy="SuperAdminOnly"),Route("api/admin/billing")]
public sealed class AdminBillingController(IBillingService service):ControllerBase
{
 [HttpGet("subscriptions")]public async Task<ActionResult<ApiResponse<PagedResult<SubscriptionDto>>>>Subscriptions([FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)=>Ok(ApiResponse<PagedResult<SubscriptionDto>>.Ok(await service.AdminSubscriptionsAsync(page,pageSize,ct)));
 [HttpPost("payments/{paymentId:guid}/refund")]public async Task<ActionResult<ApiResponse<object>>>Refund(Guid paymentId,RefundRequest request,CancellationToken ct){await service.RefundAsync(paymentId,request,ct);return Ok(ApiResponse<object>.Ok(new{},"Refund requested."));}
 [HttpPost("organizations/{organizationId:guid}/credits")]public async Task<ActionResult<ApiResponse<object>>>Credits(Guid organizationId,GrantCreditsRequest request,CancellationToken ct){await service.GrantCreditsAsync(organizationId,request,ct);return Ok(ApiResponse<object>.Ok(new{},"Credits granted."));}
 [HttpPost("organizations/{organizationId:guid}/suspend")]public async Task<ActionResult<ApiResponse<object>>>Suspend(Guid organizationId,CancellationToken ct){await service.SuspendAsync(organizationId,true,ct);return Ok(ApiResponse<object>.Ok(new{},"Organization suspended."));}
 [HttpPost("organizations/{organizationId:guid}/resume")]public async Task<ActionResult<ApiResponse<object>>>Resume(Guid organizationId,CancellationToken ct){await service.SuspendAsync(organizationId,false,ct);return Ok(ApiResponse<object>.Ok(new{},"Organization resumed."));}
 [HttpPost("coupons")]public async Task<ActionResult<ApiResponse<object>>>Coupon(CouponRequest request,CancellationToken ct){await service.CreateCouponAsync(request,ct);return Ok(ApiResponse<object>.Ok(new{},"Coupon created."));}
}
