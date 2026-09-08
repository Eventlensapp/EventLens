import type { ReactNode } from "react";

export function Icon({ children, size = 20 }: { children: ReactNode; size?: number }) {
  return <span className="icon" style={{ width: size, height: size, fontSize: size * .78 }}>{children}</span>;
}

export const CameraIcon = () => <Icon>◉</Icon>;
export const SparkleIcon = () => <Icon>✦</Icon>;
export const GalleryIcon = () => <Icon>▦</Icon>;
export const EditIcon = () => <Icon>⌁</Icon>;
export const ArrowIcon = () => <Icon>→</Icon>;
