export type AnalyticsFilter={organizationId:string;eventId?:string;from?:string;to?:string;eventType?:string;templateId?:string;photographerId?:string};
export type DashboardMetrics={totalEvents:number;activeEvents:number;totalGuests:number;totalBoothSessions:number;photosCaptured:number;aiPhotosGenerated:number;galleryViews:number;galleryVisitors:number;downloads:number;shares:number;qrScans:number;leadsCollected:number;revenue:number|null;storageUsed:number};
export type ChartPoint={label:string;value:number;secondaryValue?:number};
export type ChartData={type:"line"|"bar"|"pie"|"area"|"heatmap"|"timeline";title:string;points:ChartPoint[]};
