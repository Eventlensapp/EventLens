import{apiRequest}from"../../lib/api";
export type Checklist={id:string;title:string;description?:string;category:string;assignedUserId?:string;assignedUserName?:string;dueDate?:string;priority:string;status:string;displayOrder:number};
export type Readiness={score:number;totalTasks:number;completedTasks:number;pendingTasks:number;blockedTasks:number;criticalIssues:number;assignedStaff:number;readyBooths:number};
export type Staff={id:string;userId:string;userName:string;role:string;startDateTime:string;endDateTime:string;status:string;notes?:string};
export type Zone={id:string;venueId:string;venueName:string;name:string;description?:string;floor?:string;capacity?:number};
export type Placement={id:string;venueId:string;venueName:string;zoneId:string;zoneName:string;name:string;description?:string;positionNotes?:string;assignedOperatorId?:string;assignedOperatorName?:string;status:string};
const json=(body:unknown)=>({headers:{"Content-Type":"application/json"},body:JSON.stringify(body)});
export const operationsManagementApi={
 checklist:(e:string)=>apiRequest<Checklist[]>(`/api/events/${e}/checklists`),createTask:(e:string,b:unknown)=>apiRequest<Checklist>(`/api/events/${e}/checklists`,{method:"POST",...json(b)}),completeTask:(id:string)=>apiRequest<Checklist>(`/api/checklists/${id}/complete`,{method:"POST"}),deleteTask:(id:string)=>apiRequest(`/api/checklists/${id}`,{method:"DELETE"}),
 readiness:(e:string)=>apiRequest<Readiness>(`/api/events/${e}/readiness`),
 staff:(e:string)=>apiRequest<Staff[]>(`/api/events/${e}/staff`),assignStaff:(e:string,b:unknown)=>apiRequest<Staff>(`/api/events/${e}/staff`,{method:"POST",...json(b)}),removeStaff:(e:string,id:string)=>apiRequest(`/api/events/${e}/staff/${id}`,{method:"DELETE"}),
 zones:(e:string)=>apiRequest<Zone[]>(`/api/events/${e}/zones`),createZone:(e:string,b:unknown)=>apiRequest<Zone>(`/api/events/${e}/zones`,{method:"POST",...json(b)}),deleteZone:(id:string)=>apiRequest(`/api/zones/${id}`,{method:"DELETE"}),
 placements:(e:string)=>apiRequest<Placement[]>(`/api/events/${e}/placements`),createPlacement:(e:string,b:unknown)=>apiRequest<Placement>(`/api/events/${e}/placements`,{method:"POST",...json(b)}),deletePlacement:(id:string)=>apiRequest(`/api/placements/${id}`,{method:"DELETE"})
};
