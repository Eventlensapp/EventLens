using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;
namespace EventLensAI.UnitTests;
public sealed class EventOperationsTests
{
 [Fact]public void Venue_rejects_invalid_coordinates(){var x=new Venue(Guid.NewGuid(),Guid.NewGuid(),"Hall");Assert.Throws<ArgumentException>(()=>x.Update(Guid.NewGuid(),"Hall",null,null,null,null,null,null,91,null,null,null,null,null,null,false));}
 [Fact]public void Venue_can_be_archived_and_restored(){var actor=Guid.NewGuid();var x=new Venue(Guid.NewGuid(),Guid.NewGuid(),"Hall");x.Archive(actor);x.Restore(actor);Assert.False(x.IsDeleted);}
 [Fact]public void Schedule_rejects_end_before_start(){var now=DateTime.UtcNow;Assert.Throws<ArgumentException>(()=>new EventScheduleItem(Guid.NewGuid(),Guid.NewGuid(),"Ceremony",now,now.AddMinutes(-1),0));}
 [Fact]public void Schedule_can_be_reordered(){var now=DateTime.UtcNow;var x=new EventScheduleItem(Guid.NewGuid(),Guid.NewGuid(),"Session",now,now.AddHours(1),0);x.Reorder(4);Assert.Equal(4,x.DisplayOrder);}
}
