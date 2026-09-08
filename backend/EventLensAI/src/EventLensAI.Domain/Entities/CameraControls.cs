using EventLensAI.Domain.Common;
namespace EventLensAI.Domain.Entities;
public sealed class CameraProfile:BaseEntity
{
 private CameraProfile(){}public CameraProfile(Guid organizationId,string name,string cameraType,string resolution,double? zoom,double? focus,double? exposure,double? brightness,string contrast,string whiteBalance,bool torchEnabled){OrganizationId=organizationId;Update(name,cameraType,resolution,zoom,focus,exposure,brightness,contrast,whiteBalance,torchEnabled);}
 public Guid OrganizationId{get;private set;}public string Name{get;private set;}="";public string CameraType{get;private set;}="Browser";public string Resolution{get;private set;}="1920x1080";public double? Zoom{get;private set;}public double? Focus{get;private set;}public double? Exposure{get;private set;}public double? Brightness{get;private set;}public string Contrast{get;private set;}="Medium";public string WhiteBalance{get;private set;}="Auto";public bool TorchEnabled{get;private set;}
 public void Update(string name,string cameraType,string resolution,double? zoom,double? focus,double? exposure,double? brightness,string contrast,string whiteBalance,bool torchEnabled){Name=name.Trim();CameraType=cameraType;Resolution=resolution;Zoom=zoom;Focus=focus;Exposure=exposure;Brightness=brightness;Contrast=contrast;WhiteBalance=whiteBalance;TorchEnabled=torchEnabled;}
}
public sealed class CameraSettingsHistory:BaseEntity
{
 private CameraSettingsHistory(){}public CameraSettingsHistory(Guid organizationId,Guid? profileId,Guid changedBy,string previousValue,string newValue){OrganizationId=organizationId;CameraProfileId=profileId;ChangedBy=changedBy;PreviousValue=previousValue;NewValue=newValue;ChangedAt=DateTime.UtcNow;}
 public Guid OrganizationId{get;private set;}public Guid?CameraProfileId{get;private set;}public Guid ChangedBy{get;private set;}public string PreviousValue{get;private set;}="{}";public string NewValue{get;private set;}="{}";public DateTime ChangedAt{get;private set;}
}
