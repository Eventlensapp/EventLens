export type CameraState="Initializing"|"Ready"|"Streaming"|"Paused"|"Stopped"|"Disconnected"|"Error";
export type CameraDevice={deviceId:string;label:string;kind:string;groupId:string;facingMode?:string;isDefault:boolean;isAvailable:boolean};
export type CameraCapability={name:string;supported:boolean;minimum?:number;maximum?:number;step?:number;values:string[]};
export type CameraPreference={id?:string;organizationId:string;preferredCameraId:string|null;preferredResolution:string;mirrorPreview:boolean;aspectRatio:string};
export type CameraError={code:"NoCameraFound"|"PermissionDenied"|"CameraBusy"|"CameraDisconnected"|"BrowserUnsupported"|"StreamFailure"|"UnknownError";message:string;recovery:string;technical?:string};
export const resolutionProfiles=["640x480","1280x720","1920x1080","2560x1440","3840x2160"] as const;
