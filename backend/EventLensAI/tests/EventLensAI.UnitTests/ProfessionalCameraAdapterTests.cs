using EventLensAI.Application.Features.Booth;using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;
namespace EventLensAI.UnitTests;
public sealed class ProfessionalCameraAdapterTests
{
 [Theory][InlineData(CameraProviderType.Browser,true)][InlineData(CameraProviderType.Canon,false)][InlineData(CameraProviderType.Nikon,false)][InlineData(CameraProviderType.Sony,false)]public void Factory_selects_honest_provider_adapter(CameraProviderType provider,bool available){var x=new CameraAdapterFactory(new BrowserCameraAdapter()).Create(provider);Assert.Equal(provider,x.ProviderType);Assert.Equal(available,x.Available);}
 [Fact]public async Task Browser_connection_lifecycle_is_supported(){var c=Camera();var a=new BrowserCameraAdapter();await a.ConnectAsync(c,default);Assert.Equal(ProfessionalCameraStatus.Connected,c.Status);await a.DisconnectAsync(c,default);Assert.Equal(ProfessionalCameraStatus.Disconnected,c.Status);}
 [Fact]public async Task Vendor_adapter_fails_without_sdk_bridge(){var c=Camera(CameraProviderType.Canon);var a=new CameraAdapterFactory(new BrowserCameraAdapter()).Create(CameraProviderType.Canon);await Assert.ThrowsAsync<EventLensAI.Application.Exceptions.ConflictException>(()=>a.ConnectAsync(c,default));}
 [Fact]public async Task Browser_capability_mapping_preserves_existing_workflows(){var c=Camera();var x=await new BrowserCameraAdapter().GetCapabilitiesAsync(c,default);Assert.Contains("Photo",x.SupportedModes);Assert.Contains("Video",x.SupportedModes);}
 [Fact]public void Health_tracks_heartbeat_and_safe_error_code(){var c=Camera();c.Connect();c.Fail("bridge_lost");Assert.Equal(ProfessionalCameraStatus.Error,c.Status);Assert.Equal("bridge_lost",c.LastErrorCode);c.Heartbeat();Assert.Equal(ProfessionalCameraStatus.Available,c.Status);}
 [Fact]public async Task Registration_validation_rejects_unknown_connection(){var x=new RegisterProfessionalCameraRequest(Guid.NewGuid(),"Studio",CameraProviderType.Browser,"Webcam","secret","Firmware",null);Assert.False((await new RegisterProfessionalCameraRequestValidator().ValidateAsync(x)).IsValid);}
 static ProfessionalCamera Camera(CameraProviderType p=CameraProviderType.Browser)=>new(Guid.NewGuid(),"Camera",p,"Model","private-serial",p==CameraProviderType.Browser?"Browser":"Bridge",null);
}
