using EventLensAI.Application.Features.Booth;using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;
namespace EventLensAI.UnitTests;
public sealed class AdvancedMediaCaptureTests
{
 [Fact]public void Media_lifecycle_moves_recording_processing_completed(){var x=new CapturedMedia(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),CaptureMode.GIF,"clip.gif","media:session:key","image/gif",1280,720);Assert.Equal(MediaCaptureStatus.Recording,x.Status);x.Stop(2,12,1000);Assert.Equal(MediaCaptureStatus.Processing,x.Status);x.Complete();Assert.Equal(MediaCaptureStatus.Completed,x.Status);}
 [Fact]public void Queue_supports_progress_retry_failure_and_cancel(){var id=Guid.NewGuid();var q=new MediaProcessingQueue();Assert.Equal(MediaProcessingJobStatus.Queued,q.Queue(id).State);Assert.Equal(10,q.Start(id).Progress);q.Fail(id,"encode_failed");Assert.Equal(MediaProcessingJobStatus.Queued,q.Retry(id).State);Assert.Equal(MediaProcessingJobStatus.Cancelled,q.Cancel(id).State);}
 [Fact]public async Task Validation_rejects_unsafe_path_and_media_type(){var x=new MediaCaptureRequest(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),CaptureMode.Video,"x.exe","C:\\secret","application/octet-stream",1920,1080,1,100,3,.8m);Assert.False((await new MediaCaptureRequestValidator().ValidateAsync(x)).IsValid);}
 [Theory][InlineData(CaptureMode.GIF)][InlineData(CaptureMode.Boomerang)][InlineData(CaptureMode.Video)][InlineData(CaptureMode.Burst)][InlineData(CaptureMode.SlowMotion)][InlineData(CaptureMode.TimeLapse)][InlineData(CaptureMode.LivePhoto)]public void Advanced_modes_are_valid_enum_values(CaptureMode mode)=>Assert.True(Enum.IsDefined(mode));
 [Fact]public void Failed_media_has_safe_failure_state(){var x=new CapturedMedia(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),CaptureMode.Video,"clip.webm","media:key","video/webm",1280,720);x.Fail();Assert.Equal(MediaCaptureStatus.Failed,x.Status);}
}
