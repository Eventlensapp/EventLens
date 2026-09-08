using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;
using System.Security.Cryptography;
namespace EventLensAI.Domain.Entities;

public sealed class BoothExperienceConfiguration:BaseEntity
{
 private BoothExperienceConfiguration(){}
 public BoothExperienceConfiguration(Guid organizationId,Guid eventId,string welcomeMessage,int attractTimeout,string theme,string language,bool soundEnabled,bool fullscreenEnabled,BoothAnimationType animationType,string? backgroundMedia){OrganizationId=organizationId;EventId=eventId;Update(welcomeMessage,attractTimeout,theme,language,soundEnabled,fullscreenEnabled,animationType,backgroundMedia);}
 public Guid OrganizationId{get;private set;}public Guid EventId{get;private set;}public string WelcomeMessage{get;private set;}="Welcome";public int AttractTimeout{get;private set;}=60;public string Theme{get;private set;}="Corporate";public string Language{get;private set;}="en";public bool SoundEnabled{get;private set;}public bool FullscreenEnabled{get;private set;}public BoothAnimationType AnimationType{get;private set;}public string?BackgroundMedia{get;private set;}
 public void Update(string message,int timeout,string theme,string language,bool sound,bool fullscreen,BoothAnimationType animation,string?media){WelcomeMessage=message.Trim();AttractTimeout=timeout;Theme=theme.Trim();Language=language.Trim();SoundEnabled=sound;FullscreenEnabled=fullscreen;AnimationType=animation;BackgroundMedia=string.IsNullOrWhiteSpace(media)?null:media.Trim();}
}
public sealed class GuestBoothSession:BaseEntity
{
 private GuestBoothSession(){}
 public GuestBoothSession(Guid organizationId,Guid eventId,Guid?boothSessionId){OrganizationId=organizationId;EventId=eventId;BoothSessionId=boothSessionId;State=GuestBoothState.Attract;RecoveryToken=Convert.ToHexString(RandomNumberGenerator.GetBytes(32));LastActivityAt=DateTime.UtcNow;}
 public Guid OrganizationId{get;private set;}public Guid EventId{get;private set;}public Guid?BoothSessionId{get;private set;}public GuestBoothState State{get;private set;}public CaptureMode?SelectedMode{get;private set;}public string RecoveryToken{get;private set;}="";public DateTime LastActivityAt{get;private set;}public DateTime?CompletedAt{get;private set;}
 public void SelectMode(CaptureMode mode)=>SelectedMode=mode;
 public void Transition(GuestBoothState next){if(!Allowed(State,next))throw new InvalidOperationException($"Cannot move guest journey from {State} to {next}.");State=next;LastActivityAt=DateTime.UtcNow;if(next==GuestBoothState.Completed)CompletedAt=LastActivityAt;}
 public static bool Allowed(GuestBoothState from,GuestBoothState to)=>from==to||to==GuestBoothState.Error||(from,to) switch{(GuestBoothState.Attract,GuestBoothState.Welcome)=>true,(GuestBoothState.Welcome,GuestBoothState.ModeSelection)=>true,(GuestBoothState.ModeSelection,GuestBoothState.Preparation)=>true,(GuestBoothState.Preparation,GuestBoothState.Countdown)=>true,(GuestBoothState.Countdown,GuestBoothState.Capturing)=>true,(GuestBoothState.Capturing,GuestBoothState.Preview)=>true,(GuestBoothState.Preview,GuestBoothState.TemplateSelection)=>true,(GuestBoothState.Preview,GuestBoothState.Preparation)=>true,(GuestBoothState.Preview,GuestBoothState.Attract)=>true,(GuestBoothState.TemplateSelection,GuestBoothState.Rendering)=>true,(GuestBoothState.Rendering,GuestBoothState.Completed)=>true,(GuestBoothState.Completed,GuestBoothState.Attract)=>true,(GuestBoothState.Error,GuestBoothState.Attract)=>true,(GuestBoothState.Error,GuestBoothState.Preparation)=>true,_=>false};
}
public sealed class BoothExperienceLog:BaseEntity
{
 private BoothExperienceLog(){}public BoothExperienceLog(Guid organizationId,Guid eventId,Guid sessionId,GuestBoothState from,GuestBoothState to,string reason){OrganizationId=organizationId;EventId=eventId;GuestBoothSessionId=sessionId;FromState=from;ToState=to;Reason=reason;OccurredAt=DateTime.UtcNow;}
 public Guid OrganizationId{get;private set;}public Guid EventId{get;private set;}public Guid GuestBoothSessionId{get;private set;}public GuestBoothState FromState{get;private set;}public GuestBoothState ToState{get;private set;}public string Reason{get;private set;}="";public DateTime OccurredAt{get;private set;}
}
