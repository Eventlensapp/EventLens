namespace EventLensAI.Domain.Enums;
public enum AnalyticsMetric { EventCreated,GuestRegistered,BoothSessionStarted,PhotoCaptured,AIPhotoGenerated,GalleryView,GalleryVisitor,PhotoView,Download,Share,QrScan,LeadCollected,Like,Comment,Reaction,Print,Export }
public enum AnalyticsDeviceType { Unknown,Desktop,Mobile,Tablet,Kiosk }
public enum AnalyticsReportPeriod { Daily,Weekly,Monthly,Custom }
public enum AnalyticsExportFormat { Csv,Excel,Pdf }
