using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Features.Booth;
using EventLensAI.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;

namespace EventLensAI.UnitTests;

public sealed class CameraFoundationTests
{
    [Fact]
    public void Camera_manager_supports_preview_lifecycle_without_capture_state()
    {
        var id = Guid.NewGuid();
        var manager = new CameraManager(NullLogger<CameraManager>.Instance);

        Assert.Equal(CameraState.Initializing, manager.Initialize(id, "camera-a", "1920x1080", true, "16:9").State);
        Assert.Equal(CameraState.Streaming, manager.StartPreview(id).State);
        Assert.Equal(CameraState.Paused, manager.PausePreview(id).State);
        Assert.Equal(CameraState.Streaming, manager.ResumePreview(id).State);
        Assert.Equal(CameraState.Stopped, manager.StopPreview(id).State);
    }

    [Fact]
    public void Camera_manager_rejects_invalid_lifecycle_transition()
    {
        var manager = new CameraManager(NullLogger<CameraManager>.Instance);

        Assert.Throws<ConflictException>(() => manager.PausePreview(Guid.NewGuid()));
    }

    [Fact]
    public void Camera_manager_records_disconnect_and_can_restart()
    {
        var id = Guid.NewGuid();
        var manager = new CameraManager(NullLogger<CameraManager>.Instance);
        manager.Initialize(id, "camera-a", "1280x720", false, "4:3");
        manager.StartPreview(id);

        Assert.Equal(CameraState.Disconnected, manager.Fail(id, "CameraDisconnected").State);
        Assert.Equal(CameraState.Streaming, manager.Restart(id).State);
    }

    [Theory]
    [InlineData("640x480", "4:3", true)]
    [InlineData("3840x2160", "16:9", true)]
    [InlineData("800x600", "16:9", false)]
    [InlineData("1280x720", "3:2", false)]
    public async Task Camera_preference_validation_enforces_supported_profiles(string resolution, string aspectRatio, bool expected)
    {
        var request = new UpdateCameraPreferenceRequest(Guid.NewGuid(), "camera-a", resolution, true, aspectRatio);
        var result = await new UpdateCameraPreferenceValidator().ValidateAsync(request);

        Assert.Equal(expected, result.IsValid);
    }

    [Fact]
    public void Camera_preference_updates_only_configuration_metadata()
    {
        var preference = new CameraPreference(Guid.NewGuid(), "camera-a", "1280x720", true, "16:9");
        preference.Update("camera-b", "1920x1080", false, "4:3");

        Assert.Equal("camera-b", preference.PreferredCameraId);
        Assert.Equal("1920x1080", preference.PreferredResolution);
        Assert.False(preference.MirrorPreview);
        Assert.Equal("4:3", preference.AspectRatio);
    }
}
