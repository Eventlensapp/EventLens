using EventLensAI.Application.DTOs.Booth;
using EventLensAI.Application.Validators;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.UnitTests;

public sealed class BoothSessionTests
{
    [Fact]
    public void Session_tracks_capture_and_completion()
    {
        var session = new BoothSession(Guid.NewGuid(), null, CaptureMode.FourPhotoStrip, 3, null);
        session.BeginCapture();
        session.RecordCapture(4);
        session.Complete();

        Assert.Equal(4, session.PhotoCount);
        Assert.Equal(BoothSessionStatus.Completed, session.Status);
        Assert.NotNull(session.CompletedAt);
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(5, true)]
    [InlineData(10, true)]
    [InlineData(4, false)]
    public async Task Session_validator_accepts_supported_countdowns(int countdown, bool expected)
    {
        var request = new StartBoothSessionRequest(Guid.NewGuid(), null, CaptureMode.SinglePhoto, countdown, null);
        var result = await new StartBoothSessionRequestValidator().ValidateAsync(request);
        Assert.Equal(expected, result.IsValid);
    }
}
