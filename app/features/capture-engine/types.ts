export type CaptureConfiguration={id:string;organizationId:string;eventId:string;countdownDuration:number;numberOfPhotos:number;captureInterval:number;imageQuality:number;resolution:string;mirrorImage:boolean;autoCaptureEnabled:boolean};
export type CapturedPhotoMetadata={id:string;captureNumber:number;fileName:string;width:number;height:number;fileSize:number;mimeType:string;capturedAt:string;processingStatus:string};
export type LocalCapture={key:string;sessionId:string;captureNumber:number;blob:Blob;capturedAt:string;metadataSaved:boolean};
export type CaptureState="idle"|"preparing"|"countdown"|"capturing"|"saving"|"completed"|"cancelled"|"failed";
