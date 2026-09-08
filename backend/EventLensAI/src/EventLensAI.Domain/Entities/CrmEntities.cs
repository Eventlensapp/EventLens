using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class Guest : BaseEntity
{
    private Guest() { }
    public Guest(Guid organizationId, string firstName, string lastName, string? email, string? phone)
    { OrganizationId = organizationId; FirstName = firstName; LastName = lastName; Email = email; Phone = phone; LastVisit = DateTime.UtcNow; }
    public Guid OrganizationId { get; private set; }
    public string FirstName { get; private set; } = "";
    public string LastName { get; private set; } = "";
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Company { get; private set; }
    public string? JobTitle { get; private set; }
    public string? Country { get; private set; }
    public string? City { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? Gender { get; private set; }
    public string CustomFieldsJson { get; private set; } = "{}";
    public bool MarketingConsent { get; private set; }
    public string? ConsentVersion { get; private set; }
    public DateTime? ConsentDate { get; private set; }
    public string? ConsentIpAddress { get; private set; }
    public bool PrivacyPolicyAccepted { get; private set; }
    public DateTime LastVisit { get; private set; }
    public int TotalEvents { get; private set; }
    public decimal LifetimeValue { get; private set; }
    public ICollection<GuestTag> GuestTags { get; private set; } = [];
    public void Update(string firstName, string lastName, string? email, string? phone, string? company, string? jobTitle,
        string? country, string? city, DateOnly? birthDate, string? gender, string customFieldsJson)
    { FirstName=firstName; LastName=lastName; Email=email; Phone=phone; Company=company; JobTitle=jobTitle; Country=country; City=city; BirthDate=birthDate; Gender=gender; CustomFieldsJson=customFieldsJson; }
    public void RecordConsent(bool marketing, bool privacy, string version, string? ip)
    { MarketingConsent=marketing; PrivacyPolicyAccepted=privacy; ConsentVersion=version; ConsentDate=DateTime.UtcNow; ConsentIpAddress=ip; }
    public void RecordVisit() { LastVisit=DateTime.UtcNow; TotalEvents++; }
}

public sealed class LeadForm : BaseEntity
{
    private LeadForm() { }
    public LeadForm(Guid organizationId, Guid? eventId, string name, string schemaJson, string language)
    { OrganizationId=organizationId; EventId=eventId; Name=name; SchemaJson=schemaJson; Language=language; }
    public Guid OrganizationId { get; private set; }
    public Guid? EventId { get; private set; }
    public string Name { get; private set; } = "";
    public string SchemaJson { get; private set; } = "[]";
    public string Language { get; private set; } = "en";
    public bool IsActive { get; private set; } = true;
}
public sealed class LeadResponse : BaseEntity { private LeadResponse(){} public LeadResponse(Guid formId,Guid guestId,string valuesJson){LeadFormId=formId;GuestId=guestId;ValuesJson=valuesJson;} public Guid LeadFormId{get;private set;} public Guid GuestId{get;private set;} public string ValuesJson{get;private set;}="{}"; }
public sealed class GuestCheckIn : BaseEntity { private GuestCheckIn(){} public GuestCheckIn(Guid guestId,Guid eventId,CheckInMethod method){GuestId=guestId;EventId=eventId;Method=method;CheckedInAt=DateTime.UtcNow;} public Guid GuestId{get;private set;} public Guid EventId{get;private set;} public CheckInMethod Method{get;private set;} public DateTime CheckedInAt{get;private set;} }
public sealed class GuestActivity : BaseEntity { private GuestActivity(){} public GuestActivity(Guid guestId,Guid? eventId,CrmActivityType type,string? dataJson=null){GuestId=guestId;EventId=eventId;Type=type;DataJson=dataJson;} public Guid GuestId{get;private set;} public Guid? EventId{get;private set;} public CrmActivityType Type{get;private set;} public string? DataJson{get;private set;} }
public sealed class Tag : BaseEntity { private Tag(){} public Tag(Guid organizationId,string name,string color){OrganizationId=organizationId;Name=name;Color=color;} public Guid OrganizationId{get;private set;} public string Name{get;private set;}=""; public string Color{get;private set;}="#6366f1"; }
public sealed class GuestTag { public Guid GuestId{get;set;} public Guest Guest{get;set;}=null!; public Guid TagId{get;set;} public Tag Tag{get;set;}=null!; }
public sealed class Segment : BaseEntity { private Segment(){} public Segment(Guid organizationId,string name,string rulesJson){OrganizationId=organizationId;Name=name;RulesJson=rulesJson;} public Guid OrganizationId{get;private set;} public string Name{get;private set;}=""; public string RulesJson{get;private set;}="{}"; public bool IsDynamic{get;private set;}=true; }
public sealed class Campaign : BaseEntity { private Campaign(){} public Campaign(Guid organizationId,string name,CampaignChannel channel,Guid? segmentId,string contentJson){OrganizationId=organizationId;Name=name;Channel=channel;SegmentId=segmentId;ContentJson=contentJson;} public Guid OrganizationId{get;private set;} public Guid? SegmentId{get;private set;} public string Name{get;private set;}=""; public CampaignChannel Channel{get;private set;} public CampaignStatus Status{get;private set;} public string ContentJson{get;private set;}="{}"; public DateTime? ScheduledAt{get;private set;} }
public sealed class CampaignLog : BaseEntity { private CampaignLog(){} public Guid CampaignId{get;private set;} public Guid GuestId{get;private set;} public MessageStatus Status{get;private set;} public DateTime? SentAt{get;private set;} public string? ProviderMessageId{get;private set;} }
public abstract class MarketingMessage : BaseEntity { public Guid OrganizationId{get;protected set;} public Guid GuestId{get;protected set;} public Guid? CampaignId{get;protected set;} public string Destination{get;protected set;}=""; public string Content{get;protected set;}=""; public MessageStatus Status{get;protected set;} public string? ProviderMessageId{get;protected set;} }
public sealed class EmailMessage : MarketingMessage { private EmailMessage(){} }
public sealed class SmsMessage : MarketingMessage { private SmsMessage(){} }
public sealed class WhatsAppMessage : MarketingMessage { private WhatsAppMessage(){} }
public sealed class Automation : BaseEntity { private Automation(){} public Automation(Guid organizationId,string name,AutomationTrigger trigger,string conditionsJson){OrganizationId=organizationId;Name=name;Trigger=trigger;ConditionsJson=conditionsJson;} public Guid OrganizationId{get;private set;} public string Name{get;private set;}=""; public AutomationTrigger Trigger{get;private set;} public string ConditionsJson{get;private set;}="{}"; public bool IsActive{get;private set;} }
public sealed class WorkflowStep : BaseEntity { private WorkflowStep(){} public Guid AutomationId{get;private set;} public int Order{get;private set;} public WorkflowAction Action{get;private set;} public string ConfigurationJson{get;private set;}="{}"; }
public sealed class Survey : BaseEntity { private Survey(){} public Guid OrganizationId{get;private set;} public Guid? EventId{get;private set;} public string Name{get;private set;}=""; public string QuestionsJson{get;private set;}="[]"; public bool IsActive{get;private set;} }
public sealed class SurveyResponse : BaseEntity { private SurveyResponse(){} public Guid SurveyId{get;private set;} public Guid GuestId{get;private set;} public string AnswersJson{get;private set;}="{}"; }
public sealed class Coupon : BaseEntity { private Coupon(){} public Guid OrganizationId{get;private set;} public string Code{get;private set;}=""; public DiscountType DiscountType{get;private set;} public decimal Value{get;private set;} public DateTime Expiry{get;private set;} public int UsageLimit{get;private set;} public int UsageCount{get;private set;} public bool OnePerGuest{get;private set;} public bool IsReferral{get;private set;} }
public sealed class Referral : BaseEntity { private Referral(){} public Guid OrganizationId{get;private set;} public Guid ReferrerGuestId{get;private set;} public Guid? ReferredGuestId{get;private set;} public string Code{get;private set;}=""; public int Invites{get;private set;} public int Registrations{get;private set;} public int Conversions{get;private set;} public string? RewardJson{get;private set;} }
