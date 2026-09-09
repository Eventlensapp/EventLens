export type CaptureMode="Photo"|"GIF"|"Boomerang"|"Video"|"Burst"|"SlowMotion"|"TimeLapse"|"LivePhoto";export type MediaState="Ready"|"Recording"|"Processing"|"Preview"|"Completed"|"Error";
export type LocalMedia={key:string;sessionId:string;mode:CaptureMode;blob:Blob;poster?:Blob;fileName:string;duration:number;frameCount:number;width:number;height:number;createdAt:string;serverId?:string};
export type CapturedMedia={id:string;captureMode:CaptureMode;fileName:string;mimeType:string;duration:number;frameCount:number;fileSize:number;width:number;height:number;status:string;createdAt:string};
