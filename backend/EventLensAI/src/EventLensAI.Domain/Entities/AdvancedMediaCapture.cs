using EventLensAI.Domain.Common;using EventLensAI.Domain.Enums;
namespace EventLensAI.Domain.Entities;
public sealed class CapturedMedia:BaseEntity
{
 private CapturedMedia(){}public CapturedMedia(Guid organizationId,Guid eventId,Guid sessionId,CaptureMode mode,string fileName,string localKey,string mimeType,int width,int height){OrganizationId=organizationId;EventId=eventId;SessionId=sessionId;CaptureMode=mode;FileName=fileName;FilePath=localKey;MimeType=mimeType;Width=width;Height=height;Status=MediaCaptureStatus.Recording;}
 public Guid OrganizationId{get;private set;}public Guid EventId{get;private set;}public Guid SessionId{get;private set;}public CaptureMode CaptureMode{get;private set;}public string FileName{get;private set;}="";public string FilePath{get;private set;}="";public string MimeType{get;private set;}="";public double Duration{get;private set;}public int FrameCount{get;private set;}public long FileSize{get;private set;}public int Width{get;private set;}public int Height{get;private set;}public MediaCaptureStatus Status{get;private set;}
 public void Stop(double duration,int frames,long size){Duration=duration;FrameCount=frames;FileSize=size;Status=MediaCaptureStatus.Processing;}public void Complete()=>Status=MediaCaptureStatus.Completed;public void Fail()=>Status=MediaCaptureStatus.Failed;public void Cancel()=>Status=MediaCaptureStatus.Cancelled;
}
public sealed class MediaCaptureSettings:BaseEntity
{
 private MediaCaptureSettings(){}public MediaCaptureSettings(Guid organizationId,Guid eventId){OrganizationId=organizationId;EventId=eventId;}
 public Guid OrganizationId{get;private set;}public Guid EventId{get;private set;}public int FrameCount{get;private set;}=12;public int FrameIntervalMs{get;private set;}=150;public int DurationSeconds{get;private set;}=5;public decimal Quality{get;private set;}=.85m;public int TimeLapseIntervalSeconds{get;private set;}=5;public int TimeLapseDurationSeconds{get;private set;}=60;
}
public sealed class MediaProcessingJob:BaseEntity
{
 private MediaProcessingJob(){}public MediaProcessingJob(Guid organizationId,Guid mediaId){OrganizationId=organizationId;CapturedMediaId=mediaId;Status=MediaProcessingJobStatus.Queued;}
 public Guid OrganizationId{get;private set;}public Guid CapturedMediaId{get;private set;}public MediaProcessingJobStatus Status{get;private set;}public int Progress{get;private set;}public int Attempts{get;private set;}public string?ErrorCode{get;private set;}
 public void Start(){Status=MediaProcessingJobStatus.Processing;Attempts++;Progress=10;}public void Complete(){Status=MediaProcessingJobStatus.Completed;Progress=100;}public void Fail(string code){Status=MediaProcessingJobStatus.Failed;ErrorCode=code;}public void Cancel()=>Status=MediaProcessingJobStatus.Cancelled;public void Retry(){Status=MediaProcessingJobStatus.Queued;ErrorCode=null;Progress=0;}
}
