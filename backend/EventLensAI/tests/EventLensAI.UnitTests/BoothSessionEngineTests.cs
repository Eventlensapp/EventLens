using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Features.Booth;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.UnitTests;

public sealed class BoothSessionEngineTests
{
    [Fact]
    public void Managed_session_generates_secure_restore_token_and_tracks_lifecycle()
    {
        var session=Create();

        Assert.Equal(BoothSessionStatus.Created,session.Status);
        Assert.True(session.SessionToken?.Length>=40);
        session.StartManaged();session.Pause();session.Resume();session.CompleteManaged();

        Assert.Equal(BoothSessionStatus.Completed,session.Status);
        Assert.NotNull(session.EndedAt);
    }

    [Fact]
    public void Managed_session_rejects_invalid_transitions()
    {
        var session=Create();

        Assert.Throws<InvalidOperationException>(session.Pause);
        Assert.Throws<InvalidOperationException>(session.CompleteManaged);
    }

    [Fact]
    public void Recovery_requires_an_active_recent_session()
    {
        var session=Create();session.StartManaged();

        Assert.True(session.CanRecover(DateTime.UtcNow,300));
        session.CompleteManaged();
        Assert.False(session.CanRecover(DateTime.UtcNow,300));
    }

    [Fact]
    public void Runtime_rejects_idle_to_active_and_supports_operator_flow()
    {
        var id=Guid.NewGuid();var runtime=new BoothRuntimeService();
        Assert.Throws<ConflictException>(()=>runtime.Transition(id,ManagedBoothRuntimeState.Active));
        runtime.Transition(id,ManagedBoothRuntimeState.Preparing);
        runtime.Transition(id,ManagedBoothRuntimeState.Ready);
        runtime.Transition(id,ManagedBoothRuntimeState.Active);
        runtime.Transition(id,ManagedBoothRuntimeState.Paused);
        runtime.Transition(id,ManagedBoothRuntimeState.Active);
        runtime.Transition(id,ManagedBoothRuntimeState.Completing);

        Assert.Equal(ManagedBoothRuntimeState.Ready,runtime.Transition(id,ManagedBoothRuntimeState.Ready).State);
    }

    [Theory]
    [InlineData("guest@example.com",true)]
    [InlineData("not-an-email",false)]
    public async Task Session_creation_validation_protects_guest_contact_data(string email,bool expected)
    {
        var request=new CreateSessionRequest(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),BoothSessionType.Guest,"Guest",email,"{}","{}");
        var result=await new CreateSessionRequestValidator().ValidateAsync(request);
        Assert.Equal(expected,result.IsValid);
    }

    [Fact]
    public void Session_activity_contains_only_explicit_safe_metadata()
    {
        var activity=new BoothSessionActivity(Guid.NewGuid(),"Paused","{\"source\":\"operator\"}");
        Assert.Equal("Paused",activity.Action);Assert.Contains("operator",activity.Metadata);Assert.NotEqual(Guid.Empty,activity.Id);
    }

    private static BoothSession Create()=>new(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),BoothSessionType.Guest,"Guest",null,"{\"platform\":\"test\"}","{}");
}
