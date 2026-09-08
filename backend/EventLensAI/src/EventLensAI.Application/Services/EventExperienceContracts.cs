using EventLensAI.Application.DTOs.Events;
namespace EventLensAI.Application.Services;
public interface IEventBrandService{Task<EventBrandDto>GetAsync(Guid eventId,CancellationToken ct);Task<EventBrandDto>UpdateAsync(Guid eventId,UpdateEventBrandRequest request,CancellationToken ct);}
public interface IEventExperienceService{Task<EventExperienceDto>GetAsync(Guid eventId,CancellationToken ct);Task<EventExperienceDto>UpdateAsync(Guid eventId,UpdateExperienceRequest request,CancellationToken ct);}
public interface IEventAssetService{Task<IReadOnlyList<EventAssetDto>>ListAsync(Guid eventId,CancellationToken ct);Task<EventAssetDto>UploadAsync(Guid eventId,EventAssetUpload upload,CancellationToken ct);Task ArchiveAsync(Guid eventId,Guid assetId,CancellationToken ct);}
public interface IEventSponsorService{Task<IReadOnlyList<SponsorDto>>ListAsync(Guid eventId,CancellationToken ct);Task<SponsorDto>CreateAsync(Guid eventId,CreateSponsorRequest request,CancellationToken ct);Task<SponsorDto>UpdateAsync(Guid eventId,Guid id,UpdateSponsorRequest request,CancellationToken ct);Task ArchiveAsync(Guid eventId,Guid id,CancellationToken ct);}
