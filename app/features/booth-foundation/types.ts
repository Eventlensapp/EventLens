export type Capability={name:string;supported:boolean;detail:string};
export type Device={id:string;kind:string;label:string;detail:string};
export type Permission={permission:string;state:"Granted"|"Prompt"|"Denied"|"Unknown";message:string;canRetry:boolean};
export type BoothConfiguration={id:string;organizationId:string;boothName:string;boothMode:string;defaultCamera:string|null;defaultResolution:string;defaultAspectRatio:string;language:string;theme:string;countdownDefault:number;captureCountDefault:number;privacyMode:boolean;autoSave:boolean;offlineEnabled:boolean;mirrorPreview:boolean;autoRotate:boolean;fullscreenMode:boolean;debugMode:boolean};
export type Diagnostics={healthScore:number;warnings:number;errors:number;checks:{category:string;status:string;score:number;message:string;checkedAt:string}[]};
export type RuntimeReport={capabilities:Capability[];devices:Device[];permissions:Permission[];online:boolean;indexedDb:boolean;localStorage:boolean;memory?:number};
