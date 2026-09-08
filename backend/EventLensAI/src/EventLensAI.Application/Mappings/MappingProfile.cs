using AutoMapper;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Domain.Entities;
using EventLensAI.Application.Features.Booth;

namespace EventLensAI.Application.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Organization, OrganizationDto>()
            .ForCtorParam("SubscriptionPlan", option => option.MapFrom(source => source.Plan));
        CreateMap<Event, EventDto>();
        CreateMap<BoothConfiguration, BoothConfigurationDto>();
        CreateMap<CameraPreference, CameraPreferenceDto>();
        CreateMap<CaptureConfiguration, CaptureConfigurationDto>();
        CreateMap<CapturedPhoto, CapturedPhotoDto>();
        CreateMap<CameraProfile, CameraProfileDto>();
        CreateMap<CapturedMedia, CapturedMediaDto>();
        CreateMap<ProfessionalCamera, ProfessionalCameraDto>();
    }
}
