using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Features.Booth;

namespace EventLensAI.UnitTests;

public sealed class BoothFoundationTests
{
    [Fact]
    public void Runtime_state_follows_the_supported_startup_sequence()
    {
        var organizationId = Guid.NewGuid();
        var service = new BoothStateService();

        Assert.Equal(BoothRuntimeState.Initializing, service.Get(organizationId).State);
        Assert.Equal(BoothRuntimeState.CheckingPermissions, service.Transition(organizationId, BoothRuntimeState.CheckingPermissions).State);
        Assert.Equal(BoothRuntimeState.DetectingHardware, service.Transition(organizationId, BoothRuntimeState.DetectingHardware).State);
        Assert.Equal(BoothRuntimeState.Ready, service.Transition(organizationId, BoothRuntimeState.Ready).State);
        Assert.Equal(BoothRuntimeState.Busy, service.Transition(organizationId, BoothRuntimeState.Busy).State);
        Assert.Equal(BoothRuntimeState.Ready, service.Transition(organizationId, BoothRuntimeState.Ready).State);
    }

    [Fact]
    public void Runtime_state_rejects_skipping_required_startup_checks()
    {
        var service = new BoothStateService();

        Assert.Throws<ConflictException>(() =>
            service.Transition(Guid.NewGuid(), BoothRuntimeState.Ready));
    }

    [Fact]
    public async Task Configuration_validator_accepts_a_safe_foundation_configuration()
    {
        var request = ValidConfiguration();

        var result = await new UpdateBoothConfigurationValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Configuration_validator_rejects_invalid_capture_defaults()
    {
        var request = ValidConfiguration() with
        {
            DefaultResolution = "unknown",
            CountdownDefault = 31,
            CaptureCountDefault = 0
        };

        var result = await new UpdateBoothConfigurationValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.DefaultResolution));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.CountdownDefault));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.CaptureCountDefault));
    }

    [Theory]
    [InlineData("camera", true)]
    [InlineData("microphone", true)]
    [InlineData("notifications", true)]
    [InlineData("file-system", true)]
    [InlineData("geolocation", false)]
    public async Task Permission_validator_limits_requests_to_phase_zero_permissions(string permission, bool expected)
    {
        var result = await new PermissionRequestValidator()
            .ValidateAsync(new PermissionRequest(Guid.NewGuid(), permission));

        Assert.Equal(expected, result.IsValid);
    }

    private static UpdateBoothConfigurationRequest ValidConfiguration() => new(
        Guid.NewGuid(),
        "Main booth",
        "Standard",
        null,
        "1920x1080",
        "16:9",
        "en",
        "System",
        3,
        1,
        true,
        true,
        true,
        true,
        true,
        true,
        false);
}
