import type { CapturedPhoto } from "../types";

export type CameraDevice = { deviceId: string; label: string };
export type CameraOptions = {
  deviceId?: string;
  facingMode?: "user" | "environment";
  resolution?: "hd" | "fullHd";
};

export interface CameraAdapter {
  readonly kind: "browser" | "dslr";
  connect(options: CameraOptions): Promise<MediaStream>;
  disconnect(): void;
  devices(): Promise<CameraDevice[]>;
  capture(video: HTMLVideoElement, mirror: boolean): Promise<CapturedPhoto>;
}
