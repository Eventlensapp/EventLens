import type { BoothSession, CaptureMode } from "./types";
import { apiOrigin } from "../lib/api";

const baseUrl = `${apiOrigin()}/api`;
const authHeaders = () => {
  const token = typeof localStorage === "undefined" ? null : localStorage.getItem("eventlens_access_token");
  return { "Content-Type": "application/json", ...(token ? { Authorization: `Bearer ${token}` } : {}) };
};

export async function fetchBoothEvent(slug: string) {
  const response = await fetch(`${baseUrl}/booth/events/${encodeURIComponent(slug)}`);
  if (!response.ok) throw new Error("Event booth is unavailable.");
  return (await response.json()).data as { eventId:string; eventName:string; primaryColor:string; defaultCountdown:number; defaultCaptureMode:string };
}

export async function createBoothSession(eventId: string, eventSlug: string, captureMode: CaptureMode, countdown: number): Promise<BoothSession> {
  try {
    const response = await fetch(`${baseUrl}/booth/sessions`, {
      method: "POST", headers: authHeaders(),
      body: JSON.stringify({ eventId, guestId: null, captureMode: toApiMode(captureMode), countdown, templateId: null }),
    });
    if (!response.ok) throw new Error("offline");
    const data = (await response.json()).data;
    return { ...data, eventSlug, captureMode, status: "active", synced: true };
  } catch {
    return { sessionId: crypto.randomUUID(), eventId, eventSlug, status: "active",
      startedAt: new Date().toISOString(), captureMode, photoCount: 0,
      countdown: countdown as 3|5|10, templateId: null, synced: false };
  }
}

export const toApiMode = (mode: CaptureMode) => ({
  single: "SinglePhoto", strip2: "TwoPhotoStrip", strip3: "ThreePhotoStrip",
  strip4: "FourPhotoStrip", gif: "Gif", boomerang: "Boomerang", video: "ShortVideo",
})[mode];
