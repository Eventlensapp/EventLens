export type CaptureMode = "single" | "strip2" | "strip3" | "strip4" | "gif" | "boomerang" | "video";
export type SessionStatus = "idle" | "active" | "capturing" | "reviewing" | "completed";
export type CameraStatus = "idle" | "requesting" | "ready" | "denied" | "unavailable" | "disconnected" | "error";

export type CapturedPhoto = {
  id: string;
  blob: Blob;
  previewUrl: string;
  thumbnailUrl: string;
  width: number;
  height: number;
  capturedAt: number;
};

export type BoothSession = {
  sessionId: string;
  eventId: string;
  eventSlug: string;
  status: SessionStatus;
  startedAt: string;
  captureMode: CaptureMode;
  photoCount: number;
  countdown: 3 | 5 | 10;
  templateId: string | null;
  synced: boolean;
};

export const shotCountFor = (mode: CaptureMode) =>
  mode === "strip2" ? 2 : mode === "strip3" ? 3 : mode === "strip4" ? 4 :
  mode === "gif" || mode === "boomerang" ? 12 : 1;
