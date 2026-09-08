import type { CapturedPhoto } from "../types";

export type StripOptions = {
  layout: "vertical" | "horizontal";
  margin: number; spacing: number; border: number; radius: number;
  background: string; borderColor: string;
};

const load = (url: string) => new Promise<HTMLImageElement>((resolve, reject) => {
  const image = new Image(); image.onload = () => resolve(image); image.onerror = reject; image.src = url;
});

export async function generateStrip(photos: CapturedPhoto[], options: StripOptions): Promise<Blob> {
  if (!photos.length) throw new Error("No photos selected.");
  const images = await Promise.all(photos.map((p) => load(p.previewUrl)));
  const cellWidth = 1200;
  const ratio = images[0].height / images[0].width;
  const cellHeight = Math.round(cellWidth * ratio);
  const vertical = options.layout === "vertical";
  const width = vertical ? cellWidth + options.margin * 2 : cellWidth * photos.length + options.spacing * (photos.length - 1) + options.margin * 2;
  const height = vertical ? cellHeight * photos.length + options.spacing * (photos.length - 1) + options.margin * 2 : cellHeight + options.margin * 2;
  const canvas = typeof OffscreenCanvas !== "undefined" ? new OffscreenCanvas(width, height) : Object.assign(document.createElement("canvas"), { width, height });
  const context = canvas.getContext("2d");
  if (!context) throw new Error("Canvas is unavailable.");
  context.fillStyle = options.background; context.fillRect(0, 0, width, height);
  context.strokeStyle = options.borderColor; context.lineWidth = options.border;
  images.forEach((image, index) => {
    const x = options.margin + (vertical ? 0 : index * (cellWidth + options.spacing));
    const y = options.margin + (vertical ? index * (cellHeight + options.spacing) : 0);
    context.save();
    context.beginPath();
    context.roundRect(x, y, cellWidth, cellHeight, options.radius);
    context.clip();
    context.drawImage(image, x, y, cellWidth, cellHeight);
    context.restore();
    if (options.border) context.strokeRect(x, y, cellWidth, cellHeight);
  });
  return "convertToBlob" in canvas
    ? canvas.convertToBlob({ type: "image/jpeg", quality: 0.95 })
    : new Promise<Blob>((resolve, reject) => (canvas as HTMLCanvasElement).toBlob((b) => b ? resolve(b) : reject(new Error("Encoding failed.")), "image/jpeg", 0.95));
}
