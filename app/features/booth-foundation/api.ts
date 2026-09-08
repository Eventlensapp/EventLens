import{apiRequest}from"../../lib/api";import type{BoothConfiguration,Capability,Device,Diagnostics,Permission}from"./types";
const query=(organizationId:string)=>`?organizationId=${encodeURIComponent(organizationId)}`;
export const boothFoundationApi={
 configuration:(organizationId:string)=>apiRequest<BoothConfiguration>(`/api/booth/configuration${query(organizationId)}`),
 save:(body:BoothConfiguration)=>apiRequest<BoothConfiguration>("/api/booth/configuration",{method:"PUT",body:JSON.stringify(body)}),
 capabilities:(organizationId:string)=>apiRequest<Capability[]>(`/api/booth/capabilities${query(organizationId)}`),
 devices:(organizationId:string)=>apiRequest<Device[]>(`/api/booth/devices${query(organizationId)}`),
 permissions:(organizationId:string)=>apiRequest<Permission[]>(`/api/booth/permissions${query(organizationId)}`),
 requestPermission:(organizationId:string,permission:string)=>apiRequest<Permission>("/api/booth/permissions/request",{method:"POST",body:JSON.stringify({organizationId,permission})}),
 diagnostics:(organizationId:string)=>apiRequest<Diagnostics>(`/api/booth/diagnostics${query(organizationId)}`),
 state:(organizationId:string)=>apiRequest<{state:string;reason:string|null;changedAt:string}>(`/api/booth/state${query(organizationId)}`),
 offline:(organizationId:string)=>apiRequest<{supported:boolean;cacheInitialized:boolean;online:boolean;message:string}>(`/api/booth/offline${query(organizationId)}`),
};
