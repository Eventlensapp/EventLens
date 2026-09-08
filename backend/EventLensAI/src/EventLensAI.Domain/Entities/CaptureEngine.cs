using EventLensAI.Domain.Common;using EventLensAI.Domain.Enums;
namespace EventLensAI.Domain.Entities;
public sealed class CaptureConfiguration:BaseEntity
{
 private CaptureConfiguration(){}public CaptureConfiguration(Guid organizationId,Guid eventId){OrganizationId=organizationId;EventId=eventId;Update(5,4,3,0.92m,"1920x1080",true,true);}
 public Guid OrganizationId{get;private set;}public Guid EventId{get;private set;}public int CountdownDuration{get;private set;}=5;public int NumberOfPhotos{get;private set;}=4;public int CaptureInterval{get;private set;}=3;public decimal ImageQuality{get;private set;}=.92m;public string Resolution{get;private set;}="1920x1080";public bool MirrorImage{get;private set;}=true;public bool AutoCaptureEnabled{get;private set;}=true;
 public void Update(int countdown,int photos,int interval,decimal quality,string resolution,bool mirror,bool auto){CountdownDuration=countdown;NumberOfPhotos=photos;CaptureInterval=interval;ImageQuality=quality;Resolution=resolution;MirrorImage=mirror;AutoCaptureEnabled=auto;}
}
public sealed class CapturedPhoto:BaseEntity
{
 private CapturedPhoto(){}public CapturedPhoto(Guid organizationId,Guid eventId,Guid sessionId,int number,string fileName,string filePath,int width,int height,long size,string mimeType){OrganizationId=organizationId;EventId=eventId;SessionId=sessionId;CaptureNumber=number;FileName=fileName;FilePath=filePath;Width=width;Height=height;FileSize=size;MimeType=mimeType;CapturedAt=DateTime.UtcNow;ProcessingStatus=CaptureProcessingStatus.Captured;}
 public Guid OrganizationId{get;private set;}public Guid EventId{get;private set;}public Guid SessionId{get;private set;}public int CaptureNumber{get;private set;}public string FileName{get;private set;}="";public string FilePath{get;private set;}="";public int Width{get;private set;}public int Height{get;private set;}public long FileSize{get;private set;}public string MimeType{get;private set;}="image/jpeg";public DateTime CapturedAt{get;private set;}public CaptureProcessingStatus ProcessingStatus{get;private set;}
 public void Queue()=>ProcessingStatus=CaptureProcessingStatus.Queued;public void Begin()=>ProcessingStatus=CaptureProcessingStatus.Processing;public void Complete()=>ProcessingStatus=CaptureProcessingStatus.Completed;public void Fail()=>ProcessingStatus=CaptureProcessingStatus.Failed;
}
