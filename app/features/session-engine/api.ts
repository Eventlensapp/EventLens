import{apiRequest}from"../../lib/api";import type{BoothSession,RuntimeState,SessionType}from"./types";
export const sessionEngineApi={
 create:(body:{organizationId:string;eventId:string;boothId:string;sessionType:SessionType;guestName?:string;guestEmail?:string;deviceInformation:string;metadataJson:string})=>apiRequest<BoothSession>("/api/booth/sessions",{method:"POST",body:JSON.stringify(body)}),
 get:(id:string)=>apiRequest<BoothSession>(`/api/booth/sessions/${id}`),start:(id:string)=>action(id,"start"),pause:(id:string)=>action(id,"pause"),resume:(id:string)=>action(id,"resume"),complete:(id:string)=>action(id,"complete"),cancel:(id:string)=>action(id,"cancel"),
 recover:(organizationId:string,sessionToken:string)=>apiRequest<BoothSession|null>("/api/booth/sessions/recover",{method:"POST",body:JSON.stringify({organizationId,sessionToken,timeoutSeconds:300})}),
 runtime:(organizationId:string)=>apiRequest<RuntimeState>(`/api/booth/runtime/state?organizationId=${organizationId}`),reset:(organizationId:string)=>apiRequest<RuntimeState>(`/api/booth/runtime/reset?organizationId=${organizationId}`,{method:"POST"}),
};
const action=(id:string,name:string)=>apiRequest<BoothSession>(`/api/booth/sessions/${id}/${name}`,{method:"POST"});
