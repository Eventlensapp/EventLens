using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;
namespace EventLensAI.UnitTests;
public sealed class EventExperienceTests
{
 [Fact]public void Event_brand_stores_only_explicit_overrides(){var x=new EventBrandConfiguration(Guid.NewGuid());x.Update(null,null,null,null,null,"#112233",null,null,null,EventThemeMode.Inherit,null);Assert.Equal("#112233",x.PrimaryColor);Assert.Null(x.SecondaryColor);Assert.Null(x.FontFamily);}
 [Fact]public void Experience_settings_support_guest_journey(){var x=new EventExperienceSettings(Guid.NewGuid());x.Update("Welcome","Join us",null,"Smile","Ready","Photos","Moments",true,false,"{}");Assert.Equal("Welcome",x.WelcomeTitle);Assert.False(x.EnableSharing);}
 [Fact]public void Sponsor_requires_a_name(){Assert.Throws<ArgumentException>(()=>new EventSponsor(Guid.NewGuid(),"",Guid.NewGuid(),null,0));}
 [Fact]public void Asset_archive_is_soft_delete(){var x=new EventAsset(Guid.NewGuid(),"key",EventAssetType.Logo,"Logo","image/png",100,0);x.Archive(Guid.NewGuid());Assert.True(x.IsDeleted);Assert.False(x.IsActive);}
}
