using System.Text.Json;
using EventLensAI.Application.Features.Photos.PhotoProcessing;
using EventLensAI.Application.Features.Photos.Templates;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using EventLensAI.Infrastructure.ImageProcessing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace EventLensAI.UnitTests;

public sealed class PhotoProcessingTests
{
    [Fact]
    public void Photo_tracks_processing_completion()
    {
        var photo = new Photo(Guid.NewGuid(), Guid.NewGuid(), "2026/07/input.jpg", 1200, 1800, 1000, PhotoFormat.Jpeg);
        photo.BeginProcessing(PhotoProcessingType.Strip);
        photo.Complete("/storage/output.jpg", "/storage/thumb.jpg", 1200, 1800, 500, PhotoFormat.Jpeg);
        Assert.Equal(PhotoProcessingStatus.Completed, photo.Status);
        Assert.Equal(PhotoProcessingType.Strip, photo.ProcessingType);
        Assert.Equal("/storage/output.jpg", photo.ProcessedImageUrl);
    }

    [Fact]
    public async Task Renderer_generates_instagram_export()
    {
        using var image = new Image<Rgba32>(40, 60, Color.Coral);
        var input = new MemoryStream();
        await image.SaveAsJpegAsync(input); input.Position = 0;
        var document = new RenderDocument(1200, 1800, "#FFFFFF", StripLayout.Single, 0, 0, 0, "#000000", false, []);
        var output = await new ImageSharpProcessingService().RenderAsync(
            new ImageRenderInput([input], document, PhotoFormat.Jpeg, ExportPreset.Instagram, 90), CancellationToken.None);
        Assert.Equal(1080, output.Width);
        Assert.Equal(1350, output.Height);
        Assert.True(output.Length > 0);
    }

    [Fact]
    public async Task Template_validator_rejects_empty_organization()
    {
        var request = new UpsertTemplateRequest(Guid.Empty, "Wedding", TemplateCategory.Wedding, null,
            JsonSerializer.Serialize(new { width = 1200, height = 1800 }), false);
        Assert.False((await new UpsertTemplateRequestValidator().ValidateAsync(request)).IsValid);
    }

    [Theory]
    [InlineData("Owner", true)]
    [InlineData("Manager", true)]
    [InlineData("Editor", true)]
    [InlineData("Viewer", false)]
    public void Template_management_roles_are_restricted(string role, bool expected) =>
        Assert.Equal(expected, role is SystemRoles.Owner or SystemRoles.Manager or SystemRoles.Editor);
}
