using EventLensAI.Application.Features.Booth;using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;
namespace EventLensAI.UnitTests;
public sealed class CaptureEngineTests
{
 [Fact]public void Configuration_has_safe_booth_defaults(){var x=new CaptureConfiguration(Guid.NewGuid(),Guid.NewGuid());Assert.Equal(5,x.CountdownDuration);Assert.Equal(4,x.NumberOfPhotos);Assert.Equal(.92m,x.ImageQuality);Assert.True(x.MirrorImage);}
 [Fact]public void Countdown_emits_started_ticks_and_completed(){var x=new CountdownService().Create(3);Assert.Equal("CountdownStarted",x[0].Event);Assert.Equal(3,x.Count(e=>e.Event=="CountdownTick"));Assert.Equal("CountdownCompleted",x[^1].Event);}
 [Fact]public void Queue_runs_sequential_capture_lifecycle(){var id=Guid.NewGuid();var q=new CaptureQueueService();q.Prepare(id,2);q.Countdown(id);q.Capturing(id);Assert.Equal(CaptureWorkflowState.Preparing,q.Saved(id).State);q.Capturing(id);Assert.Equal(CaptureWorkflowState.Completed,q.Saved(id).State);}
 [Fact]public async Task Capture_metadata_rejects_cloud_paths_and_invalid_dimensions(){var r=new StartCaptureRequest(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),1,"photo.jpg","https://cloud/photo.jpg",0,1080,20,"image/png");Assert.False((await new StartCaptureRequestValidator().ValidateAsync(r)).IsValid);}
 [Fact]public void Captured_photo_tracks_metadata_without_a_public_blob_url(){var x=new CapturedPhoto(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),1,"photo.jpg","capture:session:key",1920,1080,1234,"image/jpeg");Assert.Equal(CaptureProcessingStatus.Captured,x.ProcessingStatus);Assert.StartsWith("capture:",x.FilePath);}
}
