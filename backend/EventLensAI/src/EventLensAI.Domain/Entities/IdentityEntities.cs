using EventLensAI.Domain.Common;

namespace EventLensAI.Domain.Entities;

public sealed class UserToken : BaseEntity
{
    private UserToken() { }
    public UserToken(Guid userId, string tokenHash, string purpose, DateTime expiresAt)
    { UserId=userId; TokenHash=tokenHash; Purpose=purpose; ExpiresAt=expiresAt; }
    public Guid UserId { get; private set; } public User User { get; private set; }=null!;
    public string TokenHash { get; private set; }=string.Empty; public string Purpose { get; private set; }=string.Empty;
    public DateTime ExpiresAt { get; private set; } public DateTime? UsedAt { get; private set; }
    public bool IsValid => UsedAt is null && ExpiresAt>DateTime.UtcNow && !IsDeleted;
    public void Use()=>UsedAt=DateTime.UtcNow;
}
public sealed class UserPreference : BaseEntity
{
    private UserPreference() { } public UserPreference(Guid userId){UserId=userId;}
    public Guid UserId{get;private set;} public string Theme{get;private set;}="system";
    public string Language{get;private set;}="en"; public bool EmailNotifications{get;private set;}=true;
    public bool SecurityNotifications{get;private set;}=true;
    public void Update(string theme,string language,bool email,bool security)
    {Theme=theme;Language=language;EmailNotifications=email;SecurityNotifications=security;}
}
public sealed class UserMfaSetting : BaseEntity
{
    private UserMfaSetting(){} public UserMfaSetting(Guid userId,string encryptedSecret){UserId=userId;SecretKey=encryptedSecret;}
    public Guid UserId{get;private set;} public bool Enabled{get;private set;} public string SecretKey{get;private set;}=string.Empty;
    public string RecoveryCodesHash{get;private set;}=string.Empty; public void Enable(string hashes){Enabled=true;RecoveryCodesHash=hashes;}
    public void Disable(){Enabled=false;RecoveryCodesHash=string.Empty;}
}
public sealed class ActivityLog : BaseEntity
{
    private ActivityLog(){} public ActivityLog(Guid? userId,string action,string description,string? ip,string? device)
    {UserId=userId;Action=action;Description=description;IpAddress=ip;Device=device;}
    public Guid? UserId{get;private set;} public string Action{get;private set;}=string.Empty;
    public string Description{get;private set;}=string.Empty; public string? IpAddress{get;private set;} public string? Device{get;private set;}
}
public sealed class ApiKey : BaseEntity
{
    private ApiKey(){} public ApiKey(Guid userId,string name,string prefix,string keyHash,DateTime? expiresAt)
    {UserId=userId;Name=name;Prefix=prefix;KeyHash=keyHash;ExpiresAt=expiresAt;}
    public Guid UserId{get;private set;} public string Name{get;private set;}=string.Empty; public string Prefix{get;private set;}=string.Empty;
    public string KeyHash{get;private set;}=string.Empty; public DateTime? ExpiresAt{get;private set;} public DateTime? RevokedAt{get;private set;}
    public DateTime? LastUsedAt{get;private set;} public bool IsActive=>RevokedAt is null&&(ExpiresAt is null||ExpiresAt>DateTime.UtcNow);
    public void Revoke()=>RevokedAt??=DateTime.UtcNow;
}
