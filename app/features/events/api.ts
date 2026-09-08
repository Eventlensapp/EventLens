import {apiRequest} from "../../lib/api";
export type EventType={id:string;name:string;description?:string;icon?:string;color:string;isSystemType:boolean;isActive:boolean;organizationId?:string};
export type EventDto={id:string;organizationId:string;name:string;slug:string;description?:string;eventType:string|number;eventTypeId?:string;branchId?:string;assignedManagerId?:string;brandProfileId?:string;venue?:string;address?:string;city?:string;country?:string;timezone:string;contactPerson?:string;contactEmail?:string;contactPhone?:string;startDate:string;endDate:string;status:string|number};
export type EventMember={userId:string;firstName:string;lastName:string;email:string;role:string};
export type EventDashboard={event:EventDto;organizationName:string;branchName?:string;eventTypeName?:string;assignedManager?:EventMember;assignedTeam:EventMember[];photoCount:number;guestCount:number;galleryStatus:string;boothStatus:string;aiJobs:number};
export type Page<T>={items:T[];page:number;pageSize:number;totalCount:number};
export const eventApi={
 list:(query:string)=>apiRequest<Page<EventDto>>(`/api/events?${query}`),
 get:(id:string)=>apiRequest<EventDto>(`/api/events/${id}`),
 dashboard:(id:string)=>apiRequest<EventDashboard>(`/api/events/${id}/dashboard`),
 types:(organizationId:string)=>apiRequest<EventType[]>(`/api/event-types?organizationId=${organizationId}`),
 createType:(body:unknown)=>apiRequest<EventType>("/api/event-types",{method:"POST",body:JSON.stringify(body)}),
 updateType:(id:string,body:unknown)=>apiRequest<EventType>(`/api/event-types/${id}`,{method:"PUT",body:JSON.stringify(body)}),
 archiveType:(id:string)=>apiRequest(`/api/event-types/${id}`,{method:"DELETE"}),
 create:(body:unknown)=>apiRequest<EventDto>("/api/events",{method:"POST",body:JSON.stringify(body)}),
 update:(id:string,body:unknown)=>apiRequest<EventDto>(`/api/events/${id}/core`,{method:"PUT",body:JSON.stringify(body)}),
 archive:(id:string)=>apiRequest<EventDto>(`/api/events/${id}/archive`,{method:"POST"}),
 restore:(id:string)=>apiRequest<EventDto>(`/api/events/${id}/restore`,{method:"POST"}),
 clone:(id:string,body:unknown)=>apiRequest<EventDto>(`/api/events/${id}/clone`,{method:"POST",body:JSON.stringify(body)}),
 members:(id:string)=>apiRequest<EventMember[]>(`/api/events/${id}/members`),
 assign:(id:string,userId:string,role:string)=>apiRequest<EventMember>(`/api/events/${id}/members`,{method:"POST",body:JSON.stringify({userId,role})}),
 role:(id:string,userId:string,role:string)=>apiRequest<EventMember>(`/api/events/${id}/members/${userId}`,{method:"PUT",body:JSON.stringify({role})}),
 remove:(id:string,userId:string)=>apiRequest(`/api/events/${id}/members/${userId}`,{method:"DELETE"})
};
