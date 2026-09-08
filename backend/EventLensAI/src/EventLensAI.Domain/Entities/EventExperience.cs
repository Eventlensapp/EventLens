using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class EventBrandConfiguration:BaseEntity
{
 private EventBrandConfiguration(){} public EventBrandConfiguration(Guid eventId){EventId=eventId;}
 public Guid EventId{get;private set;}public Event Event{get;private set;}=null!;public Guid?BrandProfileId{get;private set;}public BrandTheme?BrandProfile{get;private set;}
 public Guid?LogoAssetId{get;private set;}public Guid?BackgroundAssetId{get;private set;}public Guid?WatermarkAssetId{get;private set;}
 public string?EventDisplayName{get;private set;}public string?PrimaryColor{get;private set;}public string?SecondaryColor{get;private set;}public string?AccentColor{get;private set;}public string?FontFamily{get;private set;}
 public EventThemeMode ThemeMode{get;private set;}=EventThemeMode.Inherit;public string?ThemeOverridesJson{get;private set;}
 public void Update(Guid?profile,Guid?logo,Guid?background,Guid?watermark,string?display,string?primary,string?secondary,string?accent,string?font,EventThemeMode mode,string?theme)
 {BrandProfileId=profile;LogoAssetId=logo;BackgroundAssetId=background;WatermarkAssetId=watermark;EventDisplayName=Clean(display);PrimaryColor=Color(primary);SecondaryColor=Color(secondary);AccentColor=Color(accent);FontFamily=Clean(font);ThemeMode=mode;ThemeOverridesJson=Clean(theme);}
 static string?Clean(string?x)=>string.IsNullOrWhiteSpace(x)?null:x.Trim();static string?Color(string?x){x=Clean(x);if(x is not null&&(x.Length!=7||x[0]!='#'))throw new ArgumentException("Invalid colour.");return x?.ToUpperInvariant();}
}
public sealed class EventExperienceSettings:BaseEntity
{
 private EventExperienceSettings(){}public EventExperienceSettings(Guid eventId){EventId=eventId;}
 public Guid EventId{get;private set;}public Event Event{get;private set;}=null!;public string?WelcomeTitle{get;private set;}public string?WelcomeMessage{get;private set;}public Guid?WelcomeImageAssetId{get;private set;}
 public string?CaptureMessage{get;private set;}public string?CompletionMessage{get;private set;}public string?GalleryTitle{get;private set;}public string?GalleryDescription{get;private set;}
 public bool EnableDownload{get;private set;}=true;public bool EnableSharing{get;private set;}=true;public string ConfigurationJson{get;private set;}="{}";
 public void Update(string?title,string?welcome,Guid?image,string?capture,string?completion,string?gallery,string?galleryDescription,bool download,bool sharing,string json)
 {WelcomeTitle=Clean(title);WelcomeMessage=Clean(welcome);WelcomeImageAssetId=image;CaptureMessage=Clean(capture);CompletionMessage=Clean(completion);GalleryTitle=Clean(gallery);GalleryDescription=Clean(galleryDescription);EnableDownload=download;EnableSharing=sharing;ConfigurationJson=string.IsNullOrWhiteSpace(json)?"{}":json;}
 static string?Clean(string?x)=>string.IsNullOrWhiteSpace(x)?null:x.Trim();
}
public sealed class EventAsset:BaseEntity
{
 private EventAsset(){}public EventAsset(Guid eventId,string storageAssetId,EventAssetType type,string name,string contentType,long size,int order){EventId=eventId;StorageAssetId=storageAssetId;AssetType=type;DisplayName=name.Trim();ContentType=contentType;FileSize=size;DisplayOrder=order;}
 public Guid EventId{get;private set;}public Event Event{get;private set;}=null!;public string StorageAssetId{get;private set;}=string.Empty;public EventAssetType AssetType{get;private set;}public string DisplayName{get;private set;}=string.Empty;public string ContentType{get;private set;}=string.Empty;public long FileSize{get;private set;}public int DisplayOrder{get;private set;}public bool IsActive{get;private set;}=true;
 public void Archive(Guid actor){IsActive=false;SoftDelete(actor);}public void Reorder(int order)=>DisplayOrder=order;
}
public sealed class EventSponsor:BaseEntity
{
 private EventSponsor(){}public EventSponsor(Guid eventId,string name,Guid logoAssetId,string?website,int order){EventId=eventId;Update(name,logoAssetId,website,order,true);}
 public Guid EventId{get;private set;}public Event Event{get;private set;}=null!;public string Name{get;private set;}=string.Empty;public Guid LogoAssetId{get;private set;}public string?Website{get;private set;}public int DisplayOrder{get;private set;}public bool IsActive{get;private set;}=true;
 public void Update(string name,Guid logo,string?website,int order,bool active){Name=string.IsNullOrWhiteSpace(name)?throw new ArgumentException("Sponsor name is required."):name.Trim();LogoAssetId=logo;Website=string.IsNullOrWhiteSpace(website)?null:website.Trim();DisplayOrder=order;IsActive=active;}public void Archive(Guid actor){IsActive=false;SoftDelete(actor);}
}
