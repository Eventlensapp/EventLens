using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;
namespace EventLensAI.Application.Features.Booth;
public sealed record GuestBoothConfigurationDto(Guid EventId,string WelcomeMessage,int AttractTimeout,string Theme,string Language,bool SoundEnabled,bool FullscreenEnabled,BoothAnimationType AnimationType,string?BackgroundMedia);
public sealed record BoothExperienceDto(Guid EventId,string EventName,GuestBoothConfigurationDto Configuration);
public sealed record GuestJourneyStateDto(Guid SessionId,Guid EventId,GuestBoothState State,CaptureMode?SelectedMode,string RecoveryToken,DateTime LastActivityAt);
public sealed record SaveBoothExperienceConfigurationRequest(string WelcomeMessage,int AttractTimeout,string Theme,string Language,bool SoundEnabled,bool FullscreenEnabled,BoothAnimationType AnimationType,string?BackgroundMedia);
public sealed record StartGuestJourneyRequest(Guid EventId,Guid?BoothSessionId);
public sealed record TransitionGuestJourneyRequest(Guid SessionId,string RecoveryToken,GuestBoothState NextState,CaptureMode?SelectedMode,string?Reason);
public sealed record RecoverGuestJourneyRequest(Guid SessionId,string RecoveryToken);
public interface IBoothExperienceService{Task<BoothExperienceDto>GetAsync(Guid eventId,CancellationToken ct);Task<GuestBoothConfigurationDto>UpdateAsync(Guid eventId,SaveBoothExperienceConfigurationRequest request,CancellationToken ct);}
public interface IGuestJourneyService{Task<GuestJourneyStateDto>StartAsync(StartGuestJourneyRequest request,CancellationToken ct);Task<GuestJourneyStateDto>TransitionAsync(TransitionGuestJourneyRequest request,CancellationToken ct);Task<GuestJourneyStateDto>StateAsync(Guid sessionId,string token,CancellationToken ct);}
public interface IBoothRecoveryService{Task<GuestJourneyStateDto?>RecoverAsync(RecoverGuestJourneyRequest request,CancellationToken ct);}
public interface IGuestExperienceRepository{Task<BoothExperienceConfiguration?>ConfigurationAsync(Guid eventId,CancellationToken ct);Task<GuestBoothSession?>SessionAsync(Guid id,CancellationToken ct);Task AddConfigurationAsync(BoothExperienceConfiguration x,CancellationToken ct);Task AddSessionAsync(GuestBoothSession x,CancellationToken ct);Task AddLogAsync(BoothExperienceLog x,CancellationToken ct);}
