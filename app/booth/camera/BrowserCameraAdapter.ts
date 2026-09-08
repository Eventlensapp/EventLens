import type { CameraAdapter, CameraDevice, CameraOptions } from "./CameraAdapter";
import type { CapturedPhoto } from "../types";

const canvasToBlob = (canvas: HTMLCanvasElement, quality = 0.94) =>
  new Promise<Blob>((resolve, reject) =>
    canvas.toBlob((blob) => blob ? resolve(blob) : reject(new Error("Image encoding failed.")), "image/jpeg", quality));

export class BrowserCameraAdapter implements CameraAdapter {
  readonly kind = "browser" as const;
  private stream: MediaStream | null = null;

  async connect(options: CameraOptions) {
    if (!navigator.mediaDevices?.getUserMedia) throw new Error("CameraUnavailable");
    this.disconnect();
    const size = options.resolution === "fullHd" ? [1920, 1080] : [1280, 720];
    this.stream = await navigator.mediaDevices.getUserMedia({
      audio: false,
      video: {
        deviceId: options.deviceId ? { exact: options.deviceId } : undefined,
        facingMode: options.deviceId ? undefined : { ideal: options.facingMode ?? "user" },
        width: { ideal: size[0] },
        height: { ideal: size[1] },
      },
    });
    return this.stream;
  }

  disconnect() {
    this.stream?.getTracks().forEach((track) => track.stop());
    this.stream = null;
  }

  async devices(): Promise<CameraDevice[]> {
    const devices = await navigator.mediaDevices.enumerateDevices();
    return devices.filter((d) => d.kind === "videoinput")
      .map((d, index) => ({ deviceId: d.deviceId, label: d.label || `Camera ${index + 1}` }));
  }

  async capture(video: HTMLVideoElement, mirror: boolean): Promise<CapturedPhoto> {
    const width = video.videoWidth;
    const height = video.videoHeight;
    if (!width || !height) throw new Error("CameraNotReady");
    const canvas = document.createElement("canvas");
    canvas.width = width; canvas.height = height;
    const context = canvas.getContext("2d", { alpha: false });
    if (!context) throw new Error("CanvasUnavailable");
    if (mirror) { context.translate(width, 0); context.scale(-1, 1); }
    context.drawImage(video, 0, 0, width, height);
    const blob = await canvasToBlob(canvas);
    const thumb = document.createElement("canvas");
    thumb.width = 320; thumb.height = Math.round(320 * height / width);
    thumb.getContext("2d")?.drawImage(canvas, 0, 0, thumb.width, thumb.height);
    const thumbnail = await canvasToBlob(thumb, 0.78);
    return {
      id: crypto.randomUUID(), blob, previewUrl: URL.createObjectURL(blob),
      thumbnailUrl: URL.createObjectURL(thumbnail), width, height, capturedAt: Date.now(),
    };
  }
}
