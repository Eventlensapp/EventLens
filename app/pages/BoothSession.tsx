"use client";
import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCamera } from "../booth/camera/useCamera";
import { useBoothStore } from "../booth/store/useBoothStore";
import { shotCountFor } from "../booth/types";

const wait = (ms:number) => new Promise((resolve)=>setTimeout(resolve,ms));
const beep = () => {
  const context = new AudioContext(); const oscillator = context.createOscillator(); const gain = context.createGain();
  oscillator.connect(gain).connect(context.destination); oscillator.frequency.value=760; gain.gain.value=.05;
  oscillator.start(); oscillator.stop(context.currentTime+.09);
};

export default function BoothSession() {
  const videoRef = useRef<HTMLVideoElement>(null); const navigate=useNavigate(); const [busy,setBusy]=useState(false);
  const [progress,setProgress]=useState(0); const [flash,setFlash]=useState(false);
  const booth=useBoothStore(); const camera=useCamera(videoRef); const shots=shotCountFor(booth.captureMode);
  useEffect(()=>{ void camera.connect(); },[booth.deviceId,booth.facingMode,booth.resolution]);
  useEffect(()=>{ const key=(e:KeyboardEvent)=>{ if(e.code==="Space"){e.preventDefault();void captureSequence();} if(e.key==="Escape")navigate(`/booth/${booth.session?.eventSlug ?? "event"}`);}; addEventListener("keydown",key);return()=>removeEventListener("keydown",key);});
  async function captureSequence(){
    if(busy||booth.cameraStatus!=="ready")return; setBusy(true); booth.clearPhotos();
    for(let i=0;i<shots;i++){setProgress(i+1);for(let n=booth.countdown;n>0;n--){booth.setCountdownValue(n);beep();await wait(1000);}
      booth.setCountdownValue(null);setFlash(true);await wait(90);setFlash(false);booth.addPhoto(await camera.capture());if(i<shots-1)await wait(900);}
    setBusy(false);navigate("/preview");
  }
  return <main className={`booth-session ${flash?"flash":""}`}><header><button onClick={()=>navigate(`/booth/${booth.session?.eventSlug ?? "event"}`)}>← Exit</button><span>{progress?`Photo ${progress} of ${shots}`:"Live preview"}</span><b className={booth.session?.synced?"online":"offline"}>{booth.session?.synced?"Synced":"Offline"}</b></header>
    <div className="booth-view"><video ref={videoRef} muted playsInline autoPlay className={booth.mirror?"mirrored":""}/>{booth.countdownValue&&<div className="booth-countdown">{booth.countdownValue}</div>}
      {booth.cameraStatus!=="ready"&&<div className="camera-message"><h2>{booth.cameraStatus==="denied"?"Camera access is blocked":"Ready your camera"}</h2><p>Allow camera access to begin. Your photos stay in this browser.</p><button onClick={camera.connect}>{booth.cameraStatus==="requesting"?"Connecting…":"Enable camera"}</button></div>}
      <div className="booth-guide"/><button className="booth-shutter" aria-label="Capture photos" onClick={captureSequence} disabled={busy||booth.cameraStatus!=="ready"}><i/></button>
    </div><aside className="session-controls"><select aria-label="Camera" value={booth.deviceId ?? ""} onChange={e=>booth.setCamera({deviceId:e.target.value})}><option value="">Automatic camera</option>{camera.devices.map(d=><option value={d.deviceId} key={d.deviceId}>{d.label}</option>)}</select>
      <button onClick={()=>booth.setCamera({facingMode:booth.facingMode==="user"?"environment":"user",deviceId:undefined})}>Switch camera</button>
      <button onClick={()=>booth.setCamera({mirror:!booth.mirror})}>Mirror {booth.mirror?"on":"off"}</button>
      <button onClick={()=>booth.setCamera({resolution:booth.resolution==="hd"?"fullHd":"hd"})}>{booth.resolution==="hd"?"HD":"Full HD"}</button></aside>
    <p className="shortcut">Press Space to capture · Esc to exit</p></main>;
}
