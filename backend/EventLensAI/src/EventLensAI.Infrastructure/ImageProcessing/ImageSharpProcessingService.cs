using EventLensAI.Application.Features.Photos.PhotoProcessing;
using EventLensAI.Domain.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;

namespace EventLensAI.Infrastructure.ImageProcessing;

public sealed class ImageSharpProcessingService : IImageProcessingService
{
    public async Task<ImageRenderOutput> RenderAsync(ImageRenderInput input, CancellationToken ct)
    {
        var (width, height) = Size(input.Document, input.Preset);
        var background = Color.ParseHex(input.Document.Background);
        using var image = new Image<Rgba32>(width, height, background);
        var sources = new List<Image>();
        try
        {
            foreach (var stream in input.Sources) sources.Add(await Image.LoadAsync(stream, ct));
            var slots = Slots(width, height, input.Document, sources.Count);
            for (var index = 0; index < sources.Count; index++)
            {
                var slot = slots[index];
                sources[index].Mutate(x => x.AutoOrient().Resize(new ResizeOptions
                {
                    Size = slot.Size, Mode = ResizeMode.Crop, Position = AnchorPositionMode.Center
                }));
                image.Mutate(x => x.DrawImage(sources[index], slot.Location, 1));
            }
        }
        finally { foreach (var source in sources) source.Dispose(); }
        var output = new MemoryStream();
        if (input.Format == PhotoFormat.Png) await image.SaveAsPngAsync(output, ct);
        else if (input.Format == PhotoFormat.Pdf)
        {
            await using var jpeg = new MemoryStream();
            await image.SaveAsJpegAsync(jpeg, new JpegEncoder { Quality = input.Quality }, ct);
            output = CreateImagePdf(jpeg.ToArray(), width, height);
        }
        else await image.SaveAsJpegAsync(output, new JpegEncoder { Quality = input.Quality }, ct);
        output.Position = 0;
        var png = input.Format == PhotoFormat.Png;
        var pdf = input.Format == PhotoFormat.Pdf;
        return new(output, width, height, output.Length, pdf ? "application/pdf" : png ? "image/png" : "image/jpeg", pdf ? "pdf" : png ? "png" : "jpg");
    }
    private static (int, int) Size(RenderDocument document, ExportPreset preset) => preset switch
    {
        ExportPreset.Instagram => (1080, 1350), ExportPreset.WhatsApp => (1080, 1080),
        ExportPreset.A4Print => (2480, 3508), ExportPreset.PhotoBoothPrint => (1200, 1800),
        _ => (document.Width, document.Height)
    };
    private static IReadOnlyList<Rectangle> Slots(int width, int height, RenderDocument document, int count)
    {
        var p = document.Padding;
        var availableWidth = width - p * 2;
        var availableHeight = height - p * 2;
        var grid = document.Layout == StripLayout.Grid;
        var horizontal = document.Layout == StripLayout.Horizontal;
        var columns = grid ? 2 : horizontal ? count : 1;
        var rows = grid ? (int)Math.Ceiling(count / 2d) : horizontal ? 1 : count;
        var cellWidth = (availableWidth - document.Spacing * (columns - 1)) / columns;
        var cellHeight = (availableHeight - document.Spacing * (rows - 1)) / rows;
        return Enumerable.Range(0, count).Select(index =>
            new Rectangle(p + index % columns * (cellWidth + document.Spacing),
                p + index / columns * (cellHeight + document.Spacing), cellWidth, cellHeight)).ToArray();
    }
    private static MemoryStream CreateImagePdf(byte[] jpeg, int width, int height)
    {
        var output = new MemoryStream();
        var offsets = new List<long> { 0 };
        void Text(string value) { var bytes = Encoding.ASCII.GetBytes(value); output.Write(bytes); }
        void Object(int number, Action body)
        { offsets.Add(output.Position); Text($"{number} 0 obj\n"); body(); Text("\nendobj\n"); }
        Text("%PDF-1.4\n");
        Object(1, () => Text("<< /Type /Catalog /Pages 2 0 R >>"));
        Object(2, () => Text("<< /Type /Pages /Kids [3 0 R] /Count 1 >>"));
        Object(3, () => Text($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {width} {height}] /Resources << /XObject << /Im0 4 0 R >> >> /Contents 5 0 R >>"));
        Object(4, () => { Text($"<< /Type /XObject /Subtype /Image /Width {width} /Height {height} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {jpeg.Length} >>\nstream\n"); output.Write(jpeg); Text("\nendstream"); });
        var command = Encoding.ASCII.GetBytes($"q {width} 0 0 {height} 0 0 cm /Im0 Do Q");
        Object(5, () => { Text($"<< /Length {command.Length} >>\nstream\n"); output.Write(command); Text("\nendstream"); });
        var xref = output.Position; Text("xref\n0 6\n0000000000 65535 f \n");
        foreach (var offset in offsets.Skip(1)) Text($"{offset:0000000000} 00000 n \n");
        Text($"trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
        output.Position = 0; return output;
    }
}
