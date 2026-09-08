using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class BrandKit : BaseEntity
{
    private BrandKit() { }
    public BrandKit(Guid organizationId) => OrganizationId = organizationId;
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public string? PrimaryLogo { get; private set; } public string? DarkLogo { get; private set; }
    public string? LightLogo { get; private set; } public string? IconLogo { get; private set; } public string? Favicon { get; private set; }
    public string PrimaryColor { get; private set; }="#6C5CE7"; public string SecondaryColor { get; private set; }="#15152C";
    public string AccentColor { get; private set; }="#00D2D3"; public string BackgroundColor { get; private set; }="#FFFFFF";
    public string SurfaceColor { get; private set; }="#F6F7FB"; public string TextColor { get; private set; }="#15152C";
    public string SuccessColor { get; private set; }="#22C55E"; public string WarningColor { get; private set; }="#F59E0B"; public string DangerColor { get; private set; }="#EF4444";
    public string PrimaryFont { get; private set; }="Inter"; public string SecondaryFont { get; private set; }="Inter";
    public string HeadingFont { get; private set; }="Inter"; public string BodyFont { get; private set; }="Inter"; public decimal FontScale { get; private set; }=1m;
    public string WatermarkSettingsJson { get; private set; }="{}"; public string QrBrandingJson { get; private set; }="{}"; public string EmailBrandingJson { get; private set; }="{}";
    public void SetLogo(BrandLogoType type,string? key){switch(type){case BrandLogoType.Primary:PrimaryLogo=key;break;case BrandLogoType.Dark:DarkLogo=key;break;case BrandLogoType.Light:LightLogo=key;break;case BrandLogoType.Icon:IconLogo=key;break;case BrandLogoType.Favicon:Favicon=key;break;}}
    public void UpdateColors(string primary,string secondary,string accent,string background,string surface,string text,string success,string warning,string danger){PrimaryColor=primary;SecondaryColor=secondary;AccentColor=accent;BackgroundColor=background;SurfaceColor=surface;TextColor=text;SuccessColor=success;WarningColor=warning;DangerColor=danger;}
    public void UpdateTypography(string primary,string secondary,string heading,string body,decimal scale){PrimaryFont=primary.Trim();SecondaryFont=secondary.Trim();HeadingFont=heading.Trim();BodyFont=body.Trim();FontScale=scale;}
    public void UpdateSettings(string watermark,string qr,string email){WatermarkSettingsJson=watermark;QrBrandingJson=qr;EmailBrandingJson=email;}
}
public sealed class BrandAsset : BaseEntity
{
    private BrandAsset(){}
    public BrandAsset(Guid organizationId,string name,BrandAssetType type,string fileUrl,string mimeType,long fileSize,string? tags,int version=1,Guid? previousVersionId=null){OrganizationId=organizationId;Name=name.Trim();Type=type;FileUrl=fileUrl;MimeType=mimeType;FileSize=fileSize;Tags=tags;Version=version;PreviousVersionId=previousVersionId;}
    public Guid OrganizationId{get;private set;} public Organization Organization{get;private set;}=null!;public string Name{get;private set;}=string.Empty;public BrandAssetType Type{get;private set;}public string FileUrl{get;private set;}=string.Empty;public string MimeType{get;private set;}=string.Empty;public long FileSize{get;private set;}public string? Tags{get;private set;}public int Version{get;private set;}public Guid? PreviousVersionId{get;private set;}
}
public sealed class BrandTheme : BaseEntity
{
    private BrandTheme(){}
    public BrandTheme(Guid organizationId,string name,string? description,BrandThemeKind kind,string configurationJson){OrganizationId=organizationId;Rename(name,description);Kind=kind;ConfigurationJson=configurationJson;}
    public Guid OrganizationId{get;private set;}public Organization Organization{get;private set;}=null!;public string Name{get;private set;}=string.Empty;public string? Description{get;private set;}public BrandThemeKind Kind{get;private set;}public string ConfigurationJson{get;private set;}="{}";public bool IsDefault{get;private set;}public bool IsActive{get;private set;}
    public void Rename(string name,string? description){Name=name.Trim();Description=string.IsNullOrWhiteSpace(description)?null:description.Trim();}
    public void UpdateConfiguration(string json)=>ConfigurationJson=json;public void Activate(){IsActive=true;IsDefault=true;}public void Deactivate(){IsActive=false;IsDefault=false;}public BrandTheme Duplicate(string name)=>new(OrganizationId,name,Description,Kind,ConfigurationJson);
}
