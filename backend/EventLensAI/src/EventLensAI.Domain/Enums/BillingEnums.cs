namespace EventLensAI.Domain.Enums;
public enum BillingFeature { Organizations,Events,TeamMembers,AICredits,Storage,Galleries,CustomTemplates,CustomBranding,WhiteLabel,ApiAccess,CustomDomains,PrioritySupport,PdfExport }
public enum BillingInterval { Monthly,Yearly }
public enum InvoiceStatus { Draft,Open,Paid,Void,PastDue,Refunded }
public enum PaymentStatus { Pending,Succeeded,Failed,Refunded,PartiallyRefunded }
public enum BillingWebhookType { PaymentSucceeded,PaymentFailed,SubscriptionRenewed,SubscriptionCancelled,RefundIssued }
public enum UsageMetric { EventsCreated,PhotosCaptured,AIGenerations,StorageConsumed,GalleryViews,Downloads,Users,ApiRequests,Exports,QrScans,Templates,Uploads,BoothSessions }
