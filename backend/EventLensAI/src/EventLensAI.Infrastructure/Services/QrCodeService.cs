using EventLensAI.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace EventLensAI.Infrastructure.Services;

internal sealed class QrCodeService : IQrCodeService
{
    public string GenerateSvg(string content)
    {
        using var data = QRCodeGenerator.GenerateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        return new SvgQRCode(data).GetGraphic();
    }
    public string GenerateSvg(string content,string darkColor,string lightColor)
    {
        using var data=QRCodeGenerator.GenerateQrCode(content,QRCodeGenerator.ECCLevel.Q);
        return new SvgQRCode(data).GetGraphic(12,darkColor,lightColor,true);
    }
}
internal sealed class PublicUrlService(IConfiguration configuration) : IPublicUrlService
{
    private string BaseUrl => configuration["PublicAppBaseUrl"]?.TrimEnd('/')
        ?? throw new InvalidOperationException("PublicAppBaseUrl is not configured.");
    public string GetEventUrl(string slug) => $"{BaseUrl}/event/{slug}";
    public string GetQrImageUrl(Guid eventId) => $"{BaseUrl}/api/events/{eventId}/qr/image";
    public string GetPublicEventUrl(string token)=>$"{BaseUrl}/e/{Uri.EscapeDataString(token)}";
}
