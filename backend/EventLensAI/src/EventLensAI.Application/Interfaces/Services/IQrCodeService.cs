namespace EventLensAI.Application.Interfaces.Services;

public interface IQrCodeService
{
    string GenerateSvg(string content);
    string GenerateSvg(string content,string darkColor,string lightColor);
}
public interface IPublicUrlService
{
    string GetEventUrl(string slug);
    string GetQrImageUrl(Guid eventId);
    string GetPublicEventUrl(string token);
}
