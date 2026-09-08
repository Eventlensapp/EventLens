using EventLensAI.Application.Services;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.UnitTests;

public sealed class EventOperationsManagementTests
{
    [Fact]
    public void Checklist_item_can_be_completed_with_audit_data()
    {
        var actor = Guid.NewGuid();
        var item = new EventChecklistItem(Guid.NewGuid(), Guid.NewGuid(), "Test camera", "Technical Testing", 0);
        item.Complete(actor);
        Assert.Equal(ChecklistItemStatus.Completed, item.Status);
        Assert.Equal(actor, item.CompletedBy);
        Assert.NotNull(item.CompletedAt);
    }

    [Fact]
    public void Readiness_excludes_cancelled_tasks_and_counts_critical_issues()
    {
        var eventId = Guid.NewGuid();
        var completed = new EventChecklistItem(Guid.NewGuid(), eventId, "Venue", "Venue Setup", 0);
        completed.Complete(Guid.NewGuid());
        var critical = new EventChecklistItem(Guid.NewGuid(), eventId, "Printer", "Equipment Setup", 1);
        critical.Update("Printer", null, "Equipment Setup", null, null, ChecklistPriority.Critical, ChecklistItemStatus.Blocked, 1);
        var cancelled = new EventChecklistItem(Guid.NewGuid(), eventId, "Old task", "Other", 2);
        cancelled.Update("Old task", null, "Other", null, null, ChecklistPriority.Normal, ChecklistItemStatus.Cancelled, 2);
        var result = EventReadinessService.Calculate([completed, critical, cancelled], [], []);
        Assert.Equal(50, result.Score);
        Assert.Equal(2, result.TotalTasks);
        Assert.Equal(1, result.CriticalIssues);
        Assert.Equal(1, result.BlockedTasks);
    }

    [Fact]
    public void Staff_assignment_rejects_invalid_shift()
    {
        var now = DateTime.UtcNow;
        Assert.Throws<ArgumentException>(() => new EventStaffAssignment(Guid.NewGuid(), Guid.NewGuid(), EventStaffRole.BoothOperator, now, now));
    }

    [Fact]
    public void Booth_placement_can_be_marked_ready_and_archived()
    {
        var placement = new BoothPlacement(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Welcome booth");
        placement.Update(placement.VenueId, placement.ZoneId, placement.Name, null, "Near entrance", null, BoothPlacementStatus.Ready);
        Assert.Equal(BoothPlacementStatus.Ready, placement.Status);
        placement.Archive(Guid.NewGuid());
        Assert.True(placement.IsDeleted);
        Assert.Equal(BoothPlacementStatus.Removed, placement.Status);
    }
}
