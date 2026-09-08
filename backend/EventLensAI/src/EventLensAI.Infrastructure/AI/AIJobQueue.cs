using System.Threading.Channels;
using EventLensAI.Application.Features.AI;
namespace EventLensAI.Infrastructure.AI;
internal sealed class AIJobQueue:IAIJobQueue
{
 private readonly Channel<Guid> channel=Channel.CreateBounded<Guid>(new BoundedChannelOptions(1000){FullMode=BoundedChannelFullMode.Wait,SingleReader=false,SingleWriter=false});
 public ValueTask QueueAsync(Guid id,CancellationToken ct)=>channel.Writer.WriteAsync(id,ct);
 public ValueTask<Guid> DequeueAsync(CancellationToken ct)=>channel.Reader.ReadAsync(ct);
}
