using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;
namespace EventLensAI.Domain.Entities;

public sealed class AnalyticsRecord:BaseEntity
{
 private AnalyticsRecord(){}
 public AnalyticsRecord(Guid organizationId,AnalyticsMetric metric,DateTime occurredAt,Guid? eventId=null,Guid? guestId=null,Guid? photoId=null,Guid? templateId=null,Guid? photographerId=null,int value=1)
 {OrganizationId=organizationId;Metric=metric;OccurredAt=occurredAt;EventId=eventId;GuestId=guestId;PhotoId=photoId;TemplateId=templateId;PhotographerId=photographerId;Value=value;}
 public Guid OrganizationId{get;private set;} public Guid? EventId{get;private set;} public Guid? GuestId{get;private set;} public Guid? PhotoId{get;private set;} public Guid? TemplateId{get;private set;} public Guid? PhotographerId{get;private set;}
 public AnalyticsMetric Metric{get;private set;} public int Value{get;private set;} public DateTime OccurredAt{get;private set;}
 public string? SessionKeyHash{get;private set;} public AnalyticsDeviceType DeviceType{get;private set;} public string? Browser{get;private set;} public string? OperatingSystem{get;private set;} public string? Country{get;private set;} public string? City{get;private set;} public string? DimensionsJson{get;private set;}
 public void AddContext(string? sessionKeyHash,AnalyticsDeviceType device,string? browser,string? operatingSystem,string? country,string? city,string? dimensionsJson){SessionKeyHash=sessionKeyHash;DeviceType=device;Browser=browser;OperatingSystem=operatingSystem;Country=country;City=city;DimensionsJson=dimensionsJson;}
}
public sealed class AnalyticsDailyAggregate:BaseEntity
{
 private AnalyticsDailyAggregate(){}
 public AnalyticsDailyAggregate(Guid organizationId,DateOnly date,AnalyticsMetric metric,Guid? eventId,Guid? templateId,Guid? photographerId,int total,int uniqueVisitors)
 {OrganizationId=organizationId;Date=date;Metric=metric;EventId=eventId;TemplateId=templateId;PhotographerId=photographerId;Total=total;UniqueVisitors=uniqueVisitors;LastAggregatedAt=DateTime.UtcNow;}
 public Guid OrganizationId{get;private set;} public Guid? EventId{get;private set;} public Guid? TemplateId{get;private set;} public Guid? PhotographerId{get;private set;} public DateOnly Date{get;private set;} public AnalyticsMetric Metric{get;private set;} public int Total{get;private set;} public int UniqueVisitors{get;private set;} public DateTime LastAggregatedAt{get;private set;}
 public void Replace(int total,int unique){Total=total;UniqueVisitors=unique;LastAggregatedAt=DateTime.UtcNow;}
}
