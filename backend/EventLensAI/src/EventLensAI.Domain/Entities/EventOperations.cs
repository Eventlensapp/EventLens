using EventLensAI.Domain.Common;using EventLensAI.Domain.Enums;
namespace EventLensAI.Domain.Entities;
public sealed class VenueType:BaseEntity
{
 private VenueType(){}public VenueType(string name,string?description,bool system,Guid?organizationId){if(system&&organizationId.HasValue)throw new ArgumentException("System venue types cannot have an organization.");Name=Required(name);Description=Clean(description);IsSystemType=system;OrganizationId=organizationId;}
 public Guid?OrganizationId{get;private set;}public Organization?Organization{get;private set;}public string Name{get;private set;}=string.Empty;public string?Description{get;private set;}public bool IsSystemType{get;private set;}public bool IsActive{get;private set;}=true;
 static string Required(string x)=>string.IsNullOrWhiteSpace(x)?throw new ArgumentException("Name is required."):x.Trim();static string?Clean(string?x)=>string.IsNullOrWhiteSpace(x)?null:x.Trim();
}
public sealed class ScheduleType:BaseEntity
{
 private ScheduleType(){}public ScheduleType(string name,string?description,bool system,Guid?organizationId){if(system&&organizationId.HasValue)throw new ArgumentException("System schedule types cannot have an organization.");Name=string.IsNullOrWhiteSpace(name)?throw new ArgumentException("Name is required."):name.Trim();Description=string.IsNullOrWhiteSpace(description)?null:description.Trim();IsSystemType=system;OrganizationId=organizationId;}
 public Guid?OrganizationId{get;private set;}public Organization?Organization{get;private set;}public string Name{get;private set;}=string.Empty;public string?Description{get;private set;}public bool IsSystemType{get;private set;}public bool IsActive{get;private set;}=true;
}
public sealed class Venue:BaseEntity
{
 private Venue(){}public Venue(Guid eventId,Guid typeId,string name){EventId=eventId;VenueTypeId=typeId;Name=Required(name);}
 public Guid EventId{get;private set;}public Event Event{get;private set;}=null!;public Guid VenueTypeId{get;private set;}public VenueType VenueType{get;private set;}=null!;public string Name{get;private set;}=string.Empty;public string?Description{get;private set;}public string?AddressLine{get;private set;}public string?City{get;private set;}public string?State{get;private set;}public string?Country{get;private set;}public string?PostalCode{get;private set;}public decimal?Latitude{get;private set;}public decimal?Longitude{get;private set;}public string?MapProviderReference{get;private set;}public string?ContactPerson{get;private set;}public string?ContactPhone{get;private set;}public string?ContactEmail{get;private set;}public int?Capacity{get;private set;}public bool IsPrimary{get;private set;}
 public void Update(Guid type,string name,string?description,string?address,string?city,string?state,string?country,string?postal,decimal?lat,decimal?lng,string?map,string?contact,string?phone,string?email,int?capacity,bool primary){if(lat is < -90 or >90||lng is < -180 or >180)throw new ArgumentException("Invalid coordinates.");if(capacity<=0)throw new ArgumentException("Capacity must be positive.");VenueTypeId=type;Name=Required(name);Description=C(description);AddressLine=C(address);City=C(city);State=C(state);Country=C(country);PostalCode=C(postal);Latitude=lat;Longitude=lng;MapProviderReference=C(map);ContactPerson=C(contact);ContactPhone=C(phone);ContactEmail=C(email);Capacity=capacity;IsPrimary=primary;}
 public void SetPrimary(bool value)=>IsPrimary=value;public void Archive(Guid actor)=>SoftDelete(actor);static string Required(string x)=>string.IsNullOrWhiteSpace(x)?throw new ArgumentException("Venue name is required."):x.Trim();static string?C(string?x)=>string.IsNullOrWhiteSpace(x)?null:x.Trim();
}
public sealed class EventScheduleItem:BaseEntity
{
 private EventScheduleItem(){}public EventScheduleItem(Guid eventId,Guid typeId,string title,DateTime start,DateTime end,int order){EventId=eventId;Update(null,typeId,title,null,start,end,EventScheduleStatus.Planned,order);}
 public Guid EventId{get;private set;}public Event Event{get;private set;}=null!;public Guid?VenueId{get;private set;}public Venue?Venue{get;private set;}public Guid ScheduleTypeId{get;private set;}public ScheduleType ScheduleType{get;private set;}=null!;public string Title{get;private set;}=string.Empty;public string?Description{get;private set;}public DateTime StartDateTime{get;private set;}public DateTime EndDateTime{get;private set;}public EventScheduleStatus Status{get;private set;}public int DisplayOrder{get;private set;}
 public void Update(Guid?venue,Guid type,string title,string?description,DateTime start,DateTime end,EventScheduleStatus status,int order){if(end<=start)throw new ArgumentException("Schedule end time must be after start time.");VenueId=venue;ScheduleTypeId=type;Title=string.IsNullOrWhiteSpace(title)?throw new ArgumentException("Schedule title is required."):title.Trim();Description=string.IsNullOrWhiteSpace(description)?null:description.Trim();StartDateTime=start.ToUniversalTime();EndDateTime=end.ToUniversalTime();Status=status;DisplayOrder=order;}public void Reorder(int order)=>DisplayOrder=order;public void Archive(Guid actor)=>SoftDelete(actor);
}
