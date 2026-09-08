import { create } from "zustand";
import type { BoothSession, CameraStatus, CapturedPhoto, CaptureMode } from "../types";

type BoothState = {
  cameraStatus: CameraStatus; deviceId?: string; facingMode: "user" | "environment";
  resolution: "hd" | "fullHd"; mirror: boolean; countdownValue: number | null;
  photos: CapturedPhoto[]; session: BoothSession | null; selectedTemplate: string | null;
  stripUrl: string | null; captureMode: CaptureMode; countdown: 3 | 5 | 10;
  setCamera: (patch: Partial<Pick<BoothState, "cameraStatus"|"deviceId"|"facingMode"|"resolution"|"mirror">>) => void;
  setCountdownValue: (value: number | null) => void; addPhoto: (photo: CapturedPhoto) => void;
  clearPhotos: () => void; setSession: (session: BoothSession | null) => void;
  setMode: (mode: CaptureMode) => void; setCountdown: (value: 3|5|10) => void;
  setStripUrl: (url: string | null) => void;
};

export const useBoothStore = create<BoothState>((set, get) => ({
  cameraStatus: "idle", facingMode: "user", resolution: "hd", mirror: true,
  countdownValue: null, photos: [], session: null, selectedTemplate: null,
  stripUrl: null, captureMode: "strip4", countdown: 3,
  setCamera: (patch) => set(patch), setCountdownValue: (countdownValue) => set({ countdownValue }),
  addPhoto: (photo) => set({ photos: [...get().photos, photo] }),
  clearPhotos: () => { get().photos.forEach((p) => { URL.revokeObjectURL(p.previewUrl); URL.revokeObjectURL(p.thumbnailUrl); }); set({ photos: [], stripUrl: null }); },
  setSession: (session) => set({ session }), setMode: (captureMode) => set({ captureMode }),
  setCountdown: (countdown) => set({ countdown }), setStripUrl: (stripUrl) => set({ stripUrl }),
}));
