using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;
namespace EventLensAI.UnitTests;
public sealed class EventPublicAccessTests
{
 [Fact]public void Qr_token_is_valid_only_when_active_and_unexpired(){var x=new EventQRCode(Guid.NewGuid(),EventQRCodeType.EventLanding,"opaque-token","https://example.test/e/opaque-token","qr.svg","{}",DateTime.UtcNow.AddMinutes(5));Assert.True(x.IsValid(DateTime.UtcNow));x.Archive(Guid.NewGuid());Assert.False(x.IsValid(DateTime.UtcNow));}
 [Fact]public void Expired_qr_token_is_rejected_by_domain_rule(){var x=new EventQRCode(Guid.NewGuid(),EventQRCodeType.EventLanding,"opaque-token","https://example.test/e/opaque-token","qr.svg","{}",DateTime.UtcNow.AddMinutes(-1));Assert.False(x.IsValid(DateTime.UtcNow));}
 [Fact]public void Public_configuration_requires_password_hash(){var x=new EventAccessConfiguration(Guid.NewGuid());Assert.Throws<ArgumentException>(()=>x.Update(true,true,null,true,false,false,false,null));}
 [Fact]public void Disabled_public_configuration_is_unavailable(){var x=new EventAccessConfiguration(Guid.NewGuid());x.Update(false,false,null,true,false,false,false,null);Assert.False(x.IsAvailable(DateTime.UtcNow));}
 [Fact]public void Guest_session_uses_opaque_token_and_activity_timestamp(){var x=new GuestSession(Guid.NewGuid(),"session-token","127.0.0.1","Mobile");Assert.Equal("session-token",x.SessionToken);Assert.True(x.IsActive);Assert.True(x.LastActivityAt>=x.StartedAt);}
 [Fact]public void Access_log_tracks_requested_action(){var x=new EventAccessLog(Guid.NewGuid(),Guid.NewGuid(),null,EventAccessAction.QRScan,"Mobile");Assert.Equal(EventAccessAction.QRScan,x.Action);Assert.Equal("Mobile",x.DeviceType);}
}
