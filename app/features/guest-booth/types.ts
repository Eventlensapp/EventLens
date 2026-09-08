export type GuestBoothState="Attract"|"Welcome"|"ModeSelection"|"Preparation"|"Countdown"|"Capturing"|"Preview"|"TemplateSelection"|"Rendering"|"Completed"|"Error";
export type GuestMode="Photo"|"GIF"|"Boomerang"|"Video";
export type BoothConfiguration={eventId:string;welcomeMessage:string;attractTimeout:number;theme:"Wedding"|"Corporate"|"Birthday"|"Festival";language:string;soundEnabled:boolean;fullscreenEnabled:boolean;animationType:string;backgroundMedia?:string};
export type BoothExperience={eventId:string;eventName:string;configuration:BoothConfiguration};
export type GuestJourney={sessionId:string;eventId:string;state:GuestBoothState;selectedMode?:GuestMode;recoveryToken:string;lastActivityAt:string};
