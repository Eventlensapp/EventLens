using EventLensAI.Application.Features.Photos.Templates;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
namespace EventLensAI.UnitTests;
public sealed class PhotoTemplateRenderingTests
{
 [Fact]public void Template_preserves_print_dimensions_and_active_state(){var x=new PhotoTemplate(Guid.NewGuid(),"Strip","Keepsake",PhotoTemplateCategory.Wedding,1200,1800,300,"2:3",false,Guid.NewGuid());Assert.Equal(300,x.Resolution);x.SetActive(false);Assert.False(x.IsActive);}
 [Fact]public void Elements_preserve_position_rotation_and_layer(){var x=new TemplateElement(Guid.NewGuid(),TemplateElementType.Photo,20,30,400,500,12,4,"{}");Assert.Equal(20,x.PositionX);Assert.Equal(12,x.Rotation);Assert.Equal(4,x.LayerOrder);}
 [Fact]public async Task Validator_rejects_untrusted_background_and_unbounded_render_input(){var org=Guid.NewGuid();var save=new UpsertPhotoTemplateRequest(org,"x","",PhotoTemplateCategory.Brand,1200,1800,300,"2:3",false,true,TemplateLayoutType.PhotoStrip,"#15131d","https://untrusted.example/x.png",[]);Assert.False((await new UpsertPhotoTemplateRequestValidator().ValidateAsync(save)).IsValid);var render=new RenderTemplateRequest(org,Enumerable.Repeat("data:image/jpeg;base64,AA",13).ToArray(),"Event",null,DateTime.UtcNow,null,false);Assert.False((await new RenderTemplateRequestValidator().ValidateAsync(render)).IsValid);}
 [Fact]public void Supported_elements_include_photo_text_logo_sticker_date_and_qr(){var expected=new[]{TemplateElementType.Photo,TemplateElementType.Text,TemplateElementType.Logo,TemplateElementType.Sticker,TemplateElementType.Date,TemplateElementType.QRCode};Assert.All(expected,x=>Assert.True(Enum.IsDefined(x)));}
}
