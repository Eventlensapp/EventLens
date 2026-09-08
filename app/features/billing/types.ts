export type Plan={plan:"Free"|"Creator"|"Business"|"Enterprise";name:string;monthlyPrice:number;yearlyPrice:number;currency:string;maximumOrganizations:number;maximumEvents:number;maximumTeamMembers:number;monthlyAICredits:number;storageLimit:number;galleryLimit:number;templateAccess:string;customBranding:boolean;whiteLabel:boolean;apiAccess:boolean;customDomains:boolean;prioritySupport:boolean};
export type Subscription={id:string;organizationId:string;plan:string;status:string;billingInterval:string;startDate:string;endDate:string;trialEndsAt?:string;gracePeriodEndsAt?:string;cancelAtPeriodEnd:boolean;provider?:string};
export type Usage={organizationId:string;periodStart:string;usage:Record<string,number>;limits:Record<string,number>};
export type Invoice={id:string;invoiceNumber:string;customer:string;plan:string;amount:number;tax:number;discount:number;currency:string;status:string;issueDate:string;dueDate:string};
export type Payment={id:string;provider:string;providerReference:string;amount:number;currency:string;status:string;refundedAmount:number;createdAt:string};
export type Page<T>={items:T[];page:number;pageSize:number;totalCount:number;totalPages:number};
