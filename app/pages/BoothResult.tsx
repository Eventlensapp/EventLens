"use client";
import { useNavigate } from "react-router-dom";
import { useBoothStore } from "../booth/store/useBoothStore";

export default function BoothResult(){
  const booth=useBoothStore();const navigate=useNavigate();
  const download=()=>{if(!booth.stripUrl)return;const a=document.createElement("a");a.href=booth.stripUrl;a.download=`eventlens-${booth.session?.sessionId ?? "strip"}.jpg`;a.click();};
  return <main className="booth-result"><section><span>YOUR PHOTO STRIP</span><h1>That one’s a keeper.</h1><p>Your high-resolution strip is ready. Nothing has been uploaded.</p>
    {booth.stripUrl?<img src={booth.stripUrl} alt="Generated photo strip"/>:<div className="result-empty">No strip generated.</div>}
    <div><button onClick={()=>navigate("/preview")}>Back to review</button><button onClick={download}>Download</button><button onClick={()=>navigate("/photo-processing/editor")}>Continue to editor</button></div>
    <button className="new-session" onClick={()=>{booth.clearPhotos();navigate(`/booth/${booth.session?.eventSlug ?? "event"}`)}}>Start another session</button>
  </section></main>;
}
