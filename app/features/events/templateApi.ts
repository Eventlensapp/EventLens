import{apiRequest}from"../../lib/api";
export type TemplateConfiguration={boothEnabled:boolean;galleryEnabled:boolean;printingEnabled:boolean;aiEnabled:boolean;crmEnabled:boolean};
export type EventTemplate={id:string;organizationId:string;name:string;description?:string;eventTypeId:string;eventTypeName:string;defaultBrandProfileId?:string;defaultBrandProfileName?:string;defaultDurationMinutes:number;configuration:TemplateConfiguration;isSystemTemplate:boolean;isActive:boolean;createdAt:string};
export const templateApi={
 list:(o:string)=>apiRequest<EventTemplate[]>(`/api/event-templates?organizationId=${o}`),
 get:(id:string)=>apiRequest<EventTemplate>(`/api/event-templates/${id}`),
 create:(body:unknown)=>apiRequest<EventTemplate>("/api/event-templates",{method:"POST",body:JSON.stringify(body)}),
 update:(id:string,body:unknown)=>apiRequest<EventTemplate>(`/api/event-templates/${id}`,{method:"PUT",body:JSON.stringify(body)}),
 archive:(id:string)=>apiRequest(`/api/event-templates/${id}`,{method:"DELETE"}),
 clone:(id:string,name:string)=>apiRequest<EventTemplate>(`/api/event-templates/${id}/clone`,{method:"POST",body:JSON.stringify({name})})
};
