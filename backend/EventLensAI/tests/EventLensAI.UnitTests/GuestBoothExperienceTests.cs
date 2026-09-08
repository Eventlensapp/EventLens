using EventLensAI.Application.Features.Booth;using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;
namespace EventLensAI.UnitTests;
public sealed class GuestBoothExperienceTests
{
 [Fact]public void Journey_enforces_complete_guest_flow(){var x=new GuestBoothSession(Guid.NewGuid(),Guid.NewGuid(),null);foreach(var state in new[]{GuestBoothState.Welcome,GuestBoothState.ModeSelection,GuestBoothState.Preparation,GuestBoothState.Countdown,GuestBoothState.Capturing,GuestBoothState.Preview,GuestBoothState.TemplateSelection,GuestBoothState.Rendering,GuestBoothState.Completed})x.Transition(state);Assert.Equal(GuestBoothState.Completed,x.State);Assert.NotNull(x.CompletedAt);}
 [Fact]public void Journey_rejects_skipped_state(){var x=new GuestBoothSession(Guid.NewGuid(),Guid.NewGuid(),null);Assert.Throws<InvalidOperationException>(()=>x.Transition(GuestBoothState.Capturing));}
 [Fact]public void Preview_can_retake_or_cancel(){Assert.True(GuestBoothSession.Allowed(GuestBoothState.Preview,GuestBoothState.Preparation));Assert.True(GuestBoothSession.Allowed(GuestBoothState.Preview,GuestBoothState.Attract));}
 [Fact]public async Task Configuration_validation_rejects_unsafe_media_and_short_timeout(){var x=new SaveBoothExperienceConfigurationRequest("Welcome",2,"Wedding","en",true,true,BoothAnimationType.Gradient,"https://outside.test/image");Assert.False((await new SaveBoothExperienceConfigurationRequestValidator().ValidateAsync(x)).IsValid);}
 [Fact]public void Error_state_supports_restart_and_camera_retry(){Assert.True(GuestBoothSession.Allowed(GuestBoothState.Error,GuestBoothState.Attract));Assert.True(GuestBoothSession.Allowed(GuestBoothState.Error,GuestBoothState.Preparation));}
}
