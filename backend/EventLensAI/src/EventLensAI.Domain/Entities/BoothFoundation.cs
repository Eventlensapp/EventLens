using EventLensAI.Domain.Common;

namespace EventLensAI.Domain.Entities;

public sealed class BoothConfiguration : BaseEntity
{
    private BoothConfiguration() { }
    public BoothConfiguration(Guid organizationId, string boothName) { OrganizationId = organizationId; Update(boothName, "Standard", null, "1920x1080", "16:9", "en", "System", 3, 1, false, true, true, true, true, false, false); }
    public Guid OrganizationId { get; private set; }
    public string BoothName { get; private set; } = "";
    public string BoothMode { get; private set; } = "Standard";
    public string? DefaultCamera { get; private set; }
    public string DefaultResolution { get; private set; } = "1920x1080";
    public string DefaultAspectRatio { get; private set; } = "16:9";
    public string Language { get; private set; } = "en";
    public string Theme { get; private set; } = "System";
    public int CountdownDefault { get; private set; } = 3;
    public int CaptureCountDefault { get; private set; } = 1;
    public bool PrivacyMode { get; private set; }
    public bool AutoSave { get; private set; } = true;
    public bool OfflineEnabled { get; private set; } = true;
    public bool MirrorPreview { get; private set; } = true;
    public bool AutoRotate { get; private set; } = true;
    public bool FullscreenMode { get; private set; }
    public bool DebugMode { get; private set; }
    public void Update(string boothName,string boothMode,string? defaultCamera,string resolution,string aspectRatio,string language,string theme,int countdown,int captureCount,bool privacy,bool autoSave,bool offline,bool mirror,bool autoRotate,bool fullscreen,bool debug)
    {
        BoothName=boothName.Trim();BoothMode=boothMode;DefaultCamera=string.IsNullOrWhiteSpace(defaultCamera)?null:defaultCamera.Trim();
        DefaultResolution=resolution;DefaultAspectRatio=aspectRatio;Language=language;Theme=theme;CountdownDefault=countdown;CaptureCountDefault=captureCount;
        PrivacyMode=privacy;AutoSave=autoSave;OfflineEnabled=offline;MirrorPreview=mirror;AutoRotate=autoRotate;FullscreenMode=fullscreen;DebugMode=debug;
    }
}

public sealed class BoothCapability : BaseEntity
{
    private BoothCapability() { }
    public BoothCapability(Guid organizationId,string name,bool supported,string source){OrganizationId=organizationId;Name=name;IsSupported=supported;Source=source;DetectedAt=DateTime.UtcNow;}
    public Guid OrganizationId { get; private set; } public string Name { get; private set; }=""; public bool IsSupported { get; private set; }
    public string Source { get; private set; }="Browser"; public DateTime DetectedAt { get; private set; }
}

public sealed class BoothDevice : BaseEntity
{
    private BoothDevice() { }
    public BoothDevice(Guid organizationId,string deviceKeyHash,string kind,string label,string metadataJson){OrganizationId=organizationId;DeviceKeyHash=deviceKeyHash;Kind=kind;Label=label;MetadataJson=metadataJson;LastSeenAt=DateTime.UtcNow;}
    public Guid OrganizationId { get; private set; } public string DeviceKeyHash { get; private set; }=""; public string Kind { get; private set; }="";
    public string Label { get; private set; }=""; public string MetadataJson { get; private set; }="{}"; public DateTime LastSeenAt { get; private set; }
}

public sealed class BoothHealthCheck : BaseEntity
{
    private BoothHealthCheck() { }
    public BoothHealthCheck(Guid organizationId,string category,string status,int score,string message){OrganizationId=organizationId;Category=category;Status=status;Score=score;Message=message;CheckedAt=DateTime.UtcNow;}
    public Guid OrganizationId { get; private set; } public string Category { get; private set; }=""; public string Status { get; private set; }="Unknown";
    public int Score { get; private set; } public string Message { get; private set; }=""; public DateTime CheckedAt { get; private set; }
}

public sealed class CameraPreference : BaseEntity
{
    private CameraPreference() { }
    public CameraPreference(Guid organizationId, string? preferredCameraId, string preferredResolution, bool mirrorPreview, string aspectRatio)
    {
        OrganizationId = organizationId;
        Update(preferredCameraId, preferredResolution, mirrorPreview, aspectRatio);
    }
    public Guid OrganizationId { get; private set; }
    public string? PreferredCameraId { get; private set; }
    public string PreferredResolution { get; private set; } = "1280x720";
    public bool MirrorPreview { get; private set; } = true;
    public string AspectRatio { get; private set; } = "16:9";
    public void Update(string? preferredCameraId, string preferredResolution, bool mirrorPreview, string aspectRatio)
    {
        PreferredCameraId = string.IsNullOrWhiteSpace(preferredCameraId) ? null : preferredCameraId.Trim();
        PreferredResolution = preferredResolution;
        MirrorPreview = mirrorPreview;
        AspectRatio = aspectRatio;
    }
}
