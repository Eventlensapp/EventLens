using EventLensAI.Application.Features.Booth;using EventLensAI.Domain.Entities;
namespace EventLensAI.UnitTests;
public sealed class CameraControlTests
{
 [Fact]public void Profile_preserves_professional_settings(){var x=new CameraProfile(Guid.NewGuid(),"Indoor Wedding","Browser","1920x1080",2.0,.4,-.5,.1,"Medium","Incandescent",false);Assert.Equal("Indoor Wedding",x.Name);Assert.Equal(2,x.Zoom);Assert.False(x.TorchEnabled);}
 [Fact]public async Task Settings_validator_rejects_invalid_operator_values(){var x=new UpdateCameraControlSettingsRequest(Guid.NewGuid(),"bad",null,"invalid",null,null,null,null,"Extreme","Space","Fire",500);Assert.False((await new UpdateCameraControlSettingsRequestValidator().ValidateAsync(x)).IsValid);}
 [Fact]public async Task Profile_validator_accepts_browser_adapter_profile(){var x=new CreateCameraProfileRequest(Guid.NewGuid(),"Outdoor","Browser","3840x2160",null,null,null,null,"High","Daylight",false);Assert.True((await new CreateCameraProfileRequestValidator().ValidateAsync(x)).IsValid);}
 [Fact]public void Audit_history_records_actor_and_profile_without_device_identity(){var actor=Guid.NewGuid();var profile=Guid.NewGuid();var x=new CameraSettingsHistory(Guid.NewGuid(),profile,actor,"{}","{\"zoom\":2}");Assert.Equal(actor,x.ChangedBy);Assert.Equal(profile,x.CameraProfileId);Assert.DoesNotContain("device",x.NewValue,StringComparison.OrdinalIgnoreCase);}
 [Fact]public void Profile_can_be_soft_deleted_for_tenant_safe_crud(){var x=new CameraProfile(Guid.NewGuid(),"Corporate","Browser","1920x1080",null,null,null,null,"Low","Auto",false);x.SoftDelete(Guid.NewGuid());Assert.True(x.IsDeleted);}
}
