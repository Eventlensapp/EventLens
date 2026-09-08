using EventLensAI.Domain.Entities;

namespace EventLensAI.UnitTests;

public sealed class EventTemplateTests
{
    [Fact]
    public void System_event_type_cannot_be_archived()
    {
        var type=new EventTypeDefinition("Wedding",null,null,"#6C5CE7",true,null);
        Assert.Throws<InvalidOperationException>(()=>type.Archive(Guid.NewGuid()));
    }

    [Fact]
    public void Custom_event_type_can_be_updated_and_archived()
    {
        var type=new EventTypeDefinition("Meetup",null,"M","#112233",false,Guid.NewGuid());
        type.Update("Annual Meetup","Community event","A","#334455",true);
        type.Archive(Guid.NewGuid());
        Assert.Equal("Annual Meetup",type.Name);
        Assert.False(type.IsActive);
        Assert.True(type.IsDeleted);
    }

    [Fact]
    public void Template_duplication_copies_configuration_without_usage_history()
    {
        var organizationId=Guid.NewGuid();
        var source=new EventTemplate(organizationId,"Luxury Wedding",null,Guid.NewGuid(),Guid.NewGuid(),
            TimeSpan.FromHours(6),"""{"boothEnabled":true}""");
        var copy=source.Duplicate("Premium Wedding",organizationId);
        Assert.Equal(source.ConfigurationJson,copy.ConfigurationJson);
        Assert.Equal(source.EventTypeId,copy.EventTypeId);
        Assert.Equal(source.DefaultBrandProfileId,copy.DefaultBrandProfileId);
        Assert.Empty(copy.Usages);
        Assert.NotEqual(source.Id,copy.Id);
    }

    [Fact]
    public void Template_rejects_invalid_duration()
    {
        Assert.Throws<ArgumentException>(()=>new EventTemplate(Guid.NewGuid(),"Invalid",null,Guid.NewGuid(),null,
            TimeSpan.FromDays(32),"{}"));
    }
}
