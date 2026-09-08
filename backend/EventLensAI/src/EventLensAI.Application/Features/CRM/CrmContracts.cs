using EventLensAI.Application.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Features.CRM;

public sealed record GuestSearch(Guid OrganizationId,string? Search=null,string? Tag=null,bool? MarketingConsent=null,int Page=1,int PageSize=20);
public sealed record GuestDto(Guid Id,Guid OrganizationId,string FirstName,string LastName,string? Email,string? Phone,string? Company,string? JobTitle,string? Country,string? City,DateOnly? BirthDate,string? Gender,string CustomFieldsJson,bool MarketingConsent,bool PrivacyPolicyAccepted,DateTime LastVisit,int TotalEvents,IReadOnlyList<string> Tags);
public sealed record UpsertGuestRequest(Guid OrganizationId,string FirstName,string LastName,string? Email,string? Phone,string? Company,string? JobTitle,string? Country,string? City,DateOnly? BirthDate,string? Gender,string CustomFieldsJson,bool MarketingConsent,bool PrivacyPolicyAccepted,string ConsentVersion);
public sealed record LeadFormDto(Guid Id,Guid OrganizationId,Guid? EventId,string Name,string SchemaJson,string Language,bool IsActive);
public sealed record CreateLeadFormRequest(Guid OrganizationId,Guid? EventId,string Name,string SchemaJson,string Language="en");
public sealed record SubmitLeadResponseRequest(Guid GuestId,string ValuesJson);
public sealed record CheckInRequest(Guid GuestId,Guid EventId,CheckInMethod Method);
public sealed record ActivityDto(Guid Id,CrmActivityType Type,Guid? EventId,string? DataJson,DateTime CreatedAt);
public sealed record CrmDashboardDto(int NewLeads,int ReturningGuests,int Downloads,int GalleryViews,int CheckIns,int Campaigns,int MessagesSent,IReadOnlyList<NamedMetric> LeadSources,IReadOnlyList<NamedMetric> TopEvents);
public sealed record NamedMetric(string Name,int Value);
public sealed record CreateSegmentRequest(Guid OrganizationId,string Name,string RulesJson);
public sealed record CreateCampaignRequest(Guid OrganizationId,string Name,CampaignChannel Channel,Guid? SegmentId,string ContentJson);
public sealed record SegmentDto(Guid Id,string Name,string RulesJson,bool IsDynamic);
public sealed record CampaignDto(Guid Id,string Name,CampaignChannel Channel,CampaignStatus Status,DateTime? ScheduledAt);

public interface ICrmService
{
 Task<PagedResult<GuestDto>> SearchGuestsAsync(GuestSearch request,CancellationToken ct);
 Task<GuestDto> GetGuestAsync(Guid id,CancellationToken ct);
 Task<GuestDto> CreateGuestAsync(UpsertGuestRequest request,string? ip,CancellationToken ct);
 Task<GuestDto> UpdateGuestAsync(Guid id,UpsertGuestRequest request,string? ip,CancellationToken ct);
 Task<IReadOnlyList<ActivityDto>> TimelineAsync(Guid guestId,CancellationToken ct);
 Task CheckInAsync(CheckInRequest request,CancellationToken ct);
 Task<IReadOnlyList<LeadFormDto>> FormsAsync(Guid organizationId,CancellationToken ct);
 Task<LeadFormDto> CreateFormAsync(CreateLeadFormRequest request,CancellationToken ct);
 Task SubmitFormAsync(Guid formId,SubmitLeadResponseRequest request,CancellationToken ct);
 Task<CrmDashboardDto> DashboardAsync(Guid organizationId,DateTime? from,DateTime? to,CancellationToken ct);
 Task<SegmentDto> CreateSegmentAsync(CreateSegmentRequest request,CancellationToken ct);
 Task<CampaignDto> CreateCampaignAsync(CreateCampaignRequest request,CancellationToken ct);
}

public interface IEmailProvider { string Name{get;} Task<string> SendAsync(string to,string subject,string html,CancellationToken ct); }
public interface ISmsProvider { string Name{get;} Task<string> SendAsync(string to,string text,CancellationToken ct); }
public interface IWhatsAppProvider { string Name{get;} Task<string> SendAsync(string to,string template,string variablesJson,CancellationToken ct); }
