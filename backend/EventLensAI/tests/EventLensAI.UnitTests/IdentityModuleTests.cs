using EventLensAI.Domain.Entities;

namespace EventLensAI.UnitTests;

public sealed class IdentityModuleTests
{
    static User User()=>new("Test","User","test@example.com","a-valid-password-hash",null);

    [Fact] public void Five_failed_logins_lock_account()
    {var user=User();for(var i=0;i<5;i++)user.RecordFailedLogin();Assert.True(user.IsLockedOut);user.RecordLogin();Assert.False(user.IsLockedOut);}

    [Fact] public void Verification_token_is_one_use()
    {var token=new UserToken(Guid.NewGuid(),"hash","email-verification",DateTime.UtcNow.AddMinutes(5));Assert.True(token.IsValid);token.Use();Assert.False(token.IsValid);}

    [Fact] public void Expired_token_is_invalid()
    {var token=new UserToken(Guid.NewGuid(),"hash","password-reset",DateTime.UtcNow.AddSeconds(-1));Assert.False(token.IsValid);}

    [Fact] public void Preferences_can_be_updated()
    {var p=new UserPreference(Guid.NewGuid());p.Update("dark","fr",false,true);Assert.Equal("dark",p.Theme);Assert.Equal("fr",p.Language);Assert.False(p.EmailNotifications);}

    [Fact] public void Api_key_can_be_revoked()
    {var key=new ApiKey(Guid.NewGuid(),"Automation","el_example","hash",DateTime.UtcNow.AddDays(1));Assert.True(key.IsActive);key.MarkUsed();key.Revoke();Assert.NotNull(key.LastUsedAt);Assert.False(key.IsActive);}

    [Fact] public void Required_platform_and_tenant_roles_are_defined()
    {Assert.Equal("PlatformAdmin",SystemRoles.PlatformAdmin);Assert.Equal("Owner",SystemRoles.OrganizationOwner);Assert.All(new[]{SystemRoles.SuperAdmin,SystemRoles.Manager,SystemRoles.Photographer,SystemRoles.BoothOperator,SystemRoles.Designer,SystemRoles.MarketingManager,SystemRoles.Guest,SystemRoles.Viewer},x=>Assert.False(string.IsNullOrWhiteSpace(x)));}
}
