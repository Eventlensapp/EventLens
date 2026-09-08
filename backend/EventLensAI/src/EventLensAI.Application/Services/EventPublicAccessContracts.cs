using EventLensAI.Application.DTOs.Events;
namespace EventLensAI.Application.Services;
public interface IEventQRCodeService{Task<IReadOnlyList<QRCodeDto>>ListAsync(Guid eventId,CancellationToken ct);Task<QRCodeDto>CreateAsync(Guid eventId,CreateQRCodeRequest request,CancellationToken ct);Task<QRCodeDto>UpdateAsync(Guid eventId,Guid id,UpdateQRCodeRequest request,CancellationToken ct);Task ArchiveAsync(Guid eventId,Guid id,CancellationToken ct);}
public interface IPublicEventService{Task<PublicEventDto>GetAsync(string token,string?password,string?deviceType,CancellationToken ct);}
public interface IGuestSessionService{Task<GuestSessionDto>CreateAsync(string eventToken,string?password,string?ipAddress,string?deviceInfo,string?deviceType,CancellationToken ct);}
public interface IEventAccessAnalyticsService{Task<AccessAnalyticsDto>GetAsync(Guid eventId,CancellationToken ct);}
public interface IEventAccessSettingsService{Task<EventAccessSettingDto>GetAsync(Guid eventId,CancellationToken ct);Task<EventAccessSettingDto>UpdateAsync(Guid eventId,UpdateEventAccessSettingRequest request,CancellationToken ct);}
