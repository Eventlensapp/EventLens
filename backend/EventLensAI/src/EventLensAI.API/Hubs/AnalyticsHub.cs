using System.Security.Claims;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace EventLensAI.API.Hubs;
[Authorize(Policy="ViewAnalytics")]
public sealed class AnalyticsHub(EventLensDbContext db):Hub
{
 public async Task SubscribeToOrganization(Guid organizationId){var id=Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)??Context.User?.FindFirstValue("sub");if(!Guid.TryParse(id,out var userId)||!await db.OrganizationMembers.AnyAsync(x=>x.OrganizationId==organizationId&&x.UserId==userId&&!x.IsDeleted))throw new HubException("Organization access denied.");await Groups.AddToGroupAsync(Context.ConnectionId,$"analytics:organization:{organizationId}");}
 public async Task SubscribeToEvent(Guid eventId){var id=Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)??Context.User?.FindFirstValue("sub");if(!Guid.TryParse(id,out var userId)||!await db.Events.AnyAsync(e=>e.Id==eventId&&db.OrganizationMembers.Any(m=>m.OrganizationId==e.OrganizationId&&m.UserId==userId&&!m.IsDeleted)))throw new HubException("Event access denied.");await Groups.AddToGroupAsync(Context.ConnectionId,$"analytics:event:{eventId}");}
}
