"use client";
import { useQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router-dom";
import { fetchBoothEvent, createBoothSession } from "../booth/api";
import { useBoothStore } from "../booth/store/useBoothStore";
import type { CaptureMode } from "../booth/types";

const modes: { value: CaptureMode; label: string; note: string }[] = [
  { value:"single", label:"Single", note:"One perfect frame" }, { value:"strip2", label:"2 strip", note:"Two moments" },
  { value:"strip3", label:"3 strip", note:"Classic sequence" }, { value:"strip4", label:"4 strip", note:"Full photo strip" },
  { value:"gif", label:"GIF", note:"Ready for capture" }, { value:"boomerang", label:"Boomerang", note:"Ready for capture" },
  { value:"video", label:"Video", note:"Ready for capture" },
];

export default function BoothLanding() {
  const { eventSlug = "" } = useParams(); const navigate = useNavigate();
  const { captureMode, countdown, setMode, setCountdown, setSession, clearPhotos } = useBoothStore();
  const event = useQuery({ queryKey:["booth-event", eventSlug], queryFn:() => fetchBoothEvent(eventSlug), retry:1 });
  const begin = async () => {
    const eventId = event.data?.eventId ?? crypto.randomUUID();
    clearPhotos(); setSession(await createBoothSession(eventId, eventSlug, captureMode, countdown));
    navigate("/session");
  };
  return <main className="booth-home" style={{"--event-color":event.data?.primaryColor ?? "#9b87f5"} as React.CSSProperties}>
    <div className="booth-brand">EVENTLENS <span>AI</span></div>
    <section><span className="booth-kicker">PHOTO BOOTH</span><h1>{event.data?.eventName ?? (event.isError ? "Offline booth" : "Loading event…")}</h1>
      <p>Choose your experience, step into frame, and make it yours.</p>
      <div className="mode-grid">{modes.map((m) => <button key={m.value} className={captureMode===m.value?"active":""} onClick={()=>setMode(m.value)}>
        <strong>{m.label}</strong><small>{m.note}</small></button>)}</div>
      <div className="booth-options"><span>Countdown</span>{([3,5,10] as const).map((n)=><button className={countdown===n?"active":""} onClick={()=>setCountdown(n)} key={n}>{n}s</button>)}</div>
      <button className="booth-start" onClick={begin}>Start session <span>→</span></button>
    </section><footer>Camera images remain on this device until you choose to upload.</footer>
  </main>;
}
