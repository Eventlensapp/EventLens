"use client";
import { useEffect, useRef, useState } from "react";
import { AppShell } from "../components/layout/AppShell";
import { PhotoCard } from "../components/ui/PhotoCard";
import { useAppStore } from "../store/useAppStore";
const gallery = [
  ["https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=700&q=85","Lavender haze"],
  ["https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?auto=format&fit=crop&w=700&q=85","Natural light"],
  ["https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=700&q=85","Golden hour"],
  ["https://images.unsplash.com/photo-1531123897727-8f129e1688ce?auto=format&fit=crop&w=700&q=85","Film grain"],
  ["https://images.unsplash.com/photo-1488426862026-3ee34a7d66df?auto=format&fit=crop&w=700&q=85","Wild bloom"],
  ["https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=700&q=85","Midnight blue"],
];
function QR({ value }: {value:string}) { const ref=useRef<HTMLCanvasElement>(null); useEffect(()=>{const c=ref.current;if(!c)return;const x=c.getContext("2d")!,size=29,cell=6;c.width=c.height=size*cell;x.fillStyle="#fff";x.fillRect(0,0,c.width,c.height);let seed=[...value].reduce((a,ch)=>((a<<5)-a+ch.charCodeAt(0))|0,0);const finder=(ox:number,oy:number)=>{x.fillStyle="#111";x.fillRect(ox*cell,oy*cell,7*cell,7*cell);x.fillStyle="#fff";x.fillRect((ox+1)*cell,(oy+1)*cell,5*cell,5*cell);x.fillStyle="#111";x.fillRect((ox+2)*cell,(oy+2)*cell,3*cell,3*cell)};for(let row=0;row<size;row++)for(let col=0;col<size;col++){seed=(seed*1664525+1013904223)|0;if((seed>>>0)%2){x.fillStyle="#111";x.fillRect(col*cell,row*cell,cell,cell)}}finder(1,1);finder(size-8,1);finder(1,size-8)},[value]); return <canvas ref={ref} aria-label="Gallery QR code" />; }
export default function Gallery() {
  const activePhoto=useAppStore(state=>state.activePhoto); const [showQr,setShowQr]=useState(false); const [copied,setCopied]=useState(false); const shareUrl=typeof window==="undefined"?"https://prism.ai/g/sundown":`${window.location.origin}/gallery`;
  async function copy(){await navigator.clipboard?.writeText(shareUrl);setCopied(true);setTimeout(()=>setCopied(false),1500)}
  return <AppShell eyebrow="SCAN, VIEW & SHARE" title="QR Gallery"><div className="gallery-share"><div><span>LIVE EVENT GALLERY</span><h2>Studio Launch Night</h2><p>Guests can scan once and see new photos appear throughout the event.</p></div><button onClick={()=>setShowQr(true)}>▦ Show gallery QR</button></div>{activePhoto&&<section className="latest-strip"><div><span>YOUR LATEST STRIP</span><h2>Ready to share</h2></div><img src={activePhoto} alt="Latest edited strip" /></section>}<div className="gallery-toolbar"><div className="filter-pills"><button className="active">All photos</button><button>Favorites</button><button>Edited</button></div><div><button className="search">⌕ <span>Search photos</span></button><button className="upload">↑ Upload</button></div></div><div className="gallery-grid">{gallery.map(([image,title],i)=><PhotoCard key={image} image={image} title={title} tag={i%2?"Original":"AI enhanced"} tall={i===0||i===4}/>)}</div>{showQr&&<div className="modal-backdrop" onClick={()=>setShowQr(false)}><section className="qr-modal" onClick={event=>event.stopPropagation()}><button className="qr-close" onClick={()=>setShowQr(false)}>×</button><span>GUEST ACCESS</span><h2>Scan to open the gallery</h2><p>No app or account needed.</p><QR value={shareUrl}/><code>{shareUrl}</code><button className="copy-link" onClick={copy}>{copied?"Copied!":"Copy gallery link"}</button></section></div>}</AppShell>;
}
