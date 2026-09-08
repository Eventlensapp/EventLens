import{apiRequest}from"../../lib/api";import type{CameraCapability,CameraDevice,CameraPreference}from"./types";
const q=(id:string)=>`?organizationId=${encodeURIComponent(id)}`;
export const cameraFoundationApi={
 devices:(id:string)=>apiRequest<CameraDevice[]>(`/api/booth/cameras${q(id)}`),
 capabilities:(id:string)=>apiRequest<CameraCapability[]>(`/api/booth/cameras/capabilities${q(id)}`),
 preferences:(id:string)=>apiRequest<CameraPreference>(`/api/booth/cameras/preferences${q(id)}`),
 save:(body:CameraPreference)=>apiRequest<CameraPreference>("/api/booth/cameras/preferences",{method:"PUT",body:JSON.stringify(body)}),
 refresh:(id:string)=>apiRequest<CameraDevice[]>(`/api/booth/cameras/refresh${q(id)}`,{method:"POST"}),
};
