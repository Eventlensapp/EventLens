namespace EventLensAI.Domain.Enums;

public enum LeadFieldType { Text, Textarea, Email, Phone, Dropdown, Checkbox, Radio, Date, Number, ConsentCheckbox, FileUpload }
public enum CheckInMethod { Qr, Manual, Bulk, Tablet, WalkIn }
public enum CrmActivityType { Registered, CheckedIn, PhotoCaptured, GalleryViewed, PhotoDownloaded, AIUsed, EmailSent, SmsSent, WhatsAppSent, Campaign }
public enum CampaignChannel { Email, Sms, WhatsApp }
public enum CampaignStatus { Draft, Scheduled, Running, Completed, Cancelled }
public enum AutomationTrigger { PhotoCaptured, GalleryPublished, GalleryViewed, PhotoDownloaded, GuestRegistered, Birthday, Anniversary, CampaignCompleted }
public enum WorkflowAction { SendEmail, SendSms, SendWhatsApp, GenerateCoupon, NotifyManager, AddTag, AssignSegment, Webhook }
public enum SurveyQuestionType { Rating, Nps, MultipleChoice, Text, Emoji, Stars }
public enum DiscountType { Percentage, Fixed }
public enum MessageStatus { Queued, Sent, Delivered, Failed, SkippedNoConsent }
