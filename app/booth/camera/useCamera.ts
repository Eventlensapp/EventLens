import { useCallback, useEffect, useMemo, useRef, useState, type RefObject } from "react";
import { BrowserCameraAdapter } from "./BrowserCameraAdapter";
import type { CameraDevice } from "./CameraAdapter";
import { useBoothStore } from "../store/useBoothStore";

export function useCamera(videoRef: RefObject<HTMLVideoElement | null>) {
  const adapter = useMemo(() => new BrowserCameraAdapter(), []);
  const reconnectTimer = useRef<ReturnType<typeof setTimeout> | null>(null);
  const [devices, setDevices] = useState<CameraDevice[]>([]);
  const state = useBoothStore();

  const connect = useCallback(async () => {
    state.setCamera({ cameraStatus: "requesting" });
    try {
      const stream = await adapter.connect({ deviceId: state.deviceId, facingMode: state.facingMode, resolution: state.resolution });
      if (videoRef.current) { videoRef.current.srcObject = stream; await videoRef.current.play(); }
      stream.getVideoTracks()[0]?.addEventListener("ended", () => {
        state.setCamera({ cameraStatus: "disconnected" });
        reconnectTimer.current = setTimeout(connect, 1200);
      }, { once: true });
      state.setCamera({ cameraStatus: "ready" });
      setDevices(await adapter.devices());
    } catch (error) {
      const name = (error as DOMException).name;
      state.setCamera({ cameraStatus: name === "NotAllowedError" ? "denied" : name === "NotFoundError" ? "unavailable" : "error" });
    }
  }, [adapter, state.deviceId, state.facingMode, state.resolution, videoRef]);

  useEffect(() => () => { if (reconnectTimer.current) clearTimeout(reconnectTimer.current); adapter.disconnect(); }, [adapter]);
  return { connect, disconnect: () => adapter.disconnect(), devices, capture: () => {
    if (!videoRef.current) throw new Error("Camera not ready.");
    return adapter.capture(videoRef.current, state.mirror);
  }};
}
