import{create}from"zustand";import type{CameraCapability,CameraDevice,CameraError,CameraState}from"./types";
type State={state:CameraState;devices:CameraDevice[];selectedDeviceId:string;resolution:string;mirror:boolean;aspectRatio:string;capabilities:CameraCapability[];settings:MediaTrackSettings|null;error:CameraError|null;set:(value:Partial<State>)=>void};
export const useCameraFoundationStore=create<State>(set=>({state:"Stopped",devices:[],selectedDeviceId:"",resolution:"1280x720",mirror:true,aspectRatio:"16:9",capabilities:[],settings:null,error:null,set}));
