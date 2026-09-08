using System.Text.Json;
using EventLensAI.Application.Features.Photos.Templates;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace EventLensAI.Infrastructure.ImageProcessing;

public sealed class ImageSharpPhotoComposerService : IPhotoComposerService
{
    public async Task<byte[]> ComposeAsync(PhotoTemplate template,TemplateLayout layout,IReadOnlyList<TemplateElement> elements,RenderTemplateRequest request,CancellationToken ct)
    {
        var scale=request.Preview?Math.Min(1,900d/layout.CanvasWidth):1;
        var width=Math.Max(1,(int)Math.Round(layout.CanvasWidth*scale));var height=Math.Max(1,(int)Math.Round(layout.CanvasHeight*scale));
        using var canvas=new Image<Rgba32>(width,height,Color.ParseHex(layout.BackgroundColor));
        var photos=request.PhotoDataUrls.Select(Decode).Where(x=>x is not null).Cast<Image<Rgba32>>().ToArray();
        try
        {
            var photoIndex=0;
            foreach(var e in elements.OrderBy(x=>x.LayerOrder))
            {
                ct.ThrowIfCancellationRequested();
                var rect=new Rectangle((int)(e.PositionX*scale),(int)(e.PositionY*scale),Math.Max(1,(int)(e.Width*scale)),Math.Max(1,(int)(e.Height*scale)));
                if(e.ElementType==TemplateElementType.Photo&&photos.Length>0)
                {
                    using var image=photos[Math.Min(photoIndex++,photos.Length-1)].Clone(x=>x.Resize(new ResizeOptions{Size=rect.Size,Mode=ResizeMode.Crop,Position=AnchorPositionMode.Center}));
                    canvas.Mutate(x=>x.DrawImage(image,new Point(rect.X,rect.Y),1));
                }
                else if(e.ElementType==TemplateElementType.Shape)
                {
                    var fill=Color.ParseHex(Style(e.StyleConfiguration,"fill","#6C5CE7")).ToPixel<Rgba32>();Fill(canvas,rect,fill);
                }
                else if(e.ElementType==TemplateElementType.QRCode&&!string.IsNullOrWhiteSpace(request.QrContent))
                {
                    using var generator=new QRCodeGenerator();using var data=generator.CreateQrCode(request.QrContent,QRCodeGenerator.ECCLevel.Q);
                    using var qr=new PngByteQRCode(data);using var image=Image.Load<Rgba32>(qr.GetGraphic(8));
                    image.Mutate(x=>x.Resize(new ResizeOptions{Size=rect.Size,Mode=ResizeMode.Max}));
                    canvas.Mutate(x=>x.DrawImage(image,new Point(rect.X,rect.Y),1));
                }
            }
            await using var output=new MemoryStream();await canvas.SaveAsJpegAsync(output,new JpegEncoder{Quality=92},ct);return output.ToArray();
        }
        finally{foreach(var photo in photos)photo.Dispose();}
    }
    static Image<Rgba32>? Decode(string value){try{var comma=value.IndexOf(',');return comma<0?null:Image.Load<Rgba32>(Convert.FromBase64String(value[(comma+1)..]));}catch{return null;}}
    static string Style(string json,string name,string fallback){try{using var doc=JsonDocument.Parse(json);return doc.RootElement.TryGetProperty(name,out var x)?x.GetString()??fallback:fallback;}catch{return fallback;}}
    static void Fill(Image<Rgba32> image,Rectangle rect,Rgba32 color)
    {
        var left=Math.Clamp(rect.Left,0,image.Width);var right=Math.Clamp(rect.Right,0,image.Width);var top=Math.Clamp(rect.Top,0,image.Height);var bottom=Math.Clamp(rect.Bottom,0,image.Height);
        image.ProcessPixelRows(accessor=>{for(var y=top;y<bottom;y++)accessor.GetRowSpan(y)[left..right].Fill(color);});
    }
}
