using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Validators;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.UnitTests;

public sealed class EventModuleTests
{
    [Theory]
    [InlineData(EventType.Wedding)]
    [InlineData(EventType.Birthday)]
    [InlineData(EventType.Corporate)]
    [InlineData(EventType.Graduation)]
    [InlineData(EventType.School)]
    [InlineData(EventType.Exhibition)]
    [InlineData(EventType.Festival)]
    [InlineData(EventType.ProductLaunch)]
    [InlineData(EventType.Conference)]
    public void Supported_event_types_are_available_to_the_legacy_event_api(EventType type) =>
        Assert.True(Enum.IsDefined(type));

    [Fact]
    public void Event_rejects_an_end_date_before_its_start_date()
    {
        var start = DateTime.UtcNow.AddDays(1);

        Assert.Throws<ArgumentException>(() =>
            new Event(Guid.NewGuid(), "Launch", "launch", EventType.Corporate,
                start, start.AddMinutes(-1), Guid.NewGuid()));
    }

    [Fact]
    public void Event_can_be_published_then_archived()
    {
        var start = DateTime.UtcNow.AddDays(1);
        var sut = new Event(Guid.NewGuid(), "Launch", "launch", EventType.Corporate,
            start, start.AddHours(4), Guid.NewGuid());

        sut.Publish();
        Assert.Equal(EventStatus.Published, sut.Status);

        sut.Archive();
        Assert.Equal(EventStatus.Archived, sut.Status);
    }

    [Fact]
    public async Task Validator_rejects_invalid_colors_and_excessive_duration()
    {
        var start = DateTime.UtcNow.AddDays(1);
        var request = new UpsertEventRequest(
            "Launch", null, EventType.Corporate, null, null, null, null,
            start, start.AddDays(32), null, null, "blue", "#FFFFFF",
            null, null, null, true, false, true, true, false, true);

        var result = await new UpsertEventRequestValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "PrimaryColor");
        Assert.Contains(result.Errors, x => x.ErrorMessage.Contains("31 days"));
    }

    [Theory]
    [InlineData("Owner", true)]
    [InlineData("Manager", true)]
    [InlineData("Photographer", false)]
    [InlineData("Viewer", false)]
    public void Event_management_roles_are_explicit(string role, bool expected) =>
        Assert.Equal(expected, role is SystemRoles.Owner or SystemRoles.Manager);

    [Fact]
    public void Event_status_workflow_rejects_invalid_transition()
    {
        var sut=new Event(Guid.NewGuid(),"Conference","conference",EventType.Conference,
            DateTime.UtcNow.AddDays(2),DateTime.UtcNow.AddDays(3),Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(()=>sut.ChangeStatus(EventStatus.Completed));
    }

    [Fact]
    public void Archived_event_can_be_restored_without_losing_identity()
    {
        var actor=Guid.NewGuid();
        var sut=new Event(Guid.NewGuid(),"Wedding","wedding",EventType.Wedding,
            DateTime.UtcNow.AddDays(2),DateTime.UtcNow.AddDays(3),actor);
        var id=sut.Id;
        sut.Archive();sut.SoftDelete(actor);sut.RestoreFromArchive(actor);
        Assert.Equal(id,sut.Id);Assert.False(sut.IsDeleted);Assert.Equal(EventStatus.Upcoming,sut.Status);
    }

    [Fact]
    public void Event_member_role_can_be_changed()
    {
        var actor=Guid.NewGuid();
        var sut=new EventMember(Guid.NewGuid(),Guid.NewGuid(),EventMemberRole.Viewer,actor);
        sut.ChangeRole(EventMemberRole.Photographer,actor);
        Assert.Equal(EventMemberRole.Photographer,sut.Role);
    }
}
