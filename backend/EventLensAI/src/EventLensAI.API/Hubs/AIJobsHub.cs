using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace EventLensAI.API.Hubs;
[Authorize]
public sealed class AIJobsHub:Hub
{
 public Task SubscribeToEvent(Guid eventId)=>Groups.AddToGroupAsync(Context.ConnectionId,$"event:{eventId}");
 public Task UnsubscribeFromEvent(Guid eventId)=>Groups.RemoveFromGroupAsync(Context.ConnectionId,$"event:{eventId}");
}
