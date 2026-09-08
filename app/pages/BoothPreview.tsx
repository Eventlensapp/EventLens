"use client";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useBoothStore } from "../booth/store/useBoothStore";
import { generateStrip, type StripOptions } from "../booth/processing/stripGenerator";

export default function BoothPreview(){
  const booth=useBoothStore();const navigate=useNavigate();const [working,setWorking]=useState(false);
  const [options,setOptions]=useState<StripOptions>({layout:"vertical",margin:54,spacing:24,border:0,radius:20,background:"#ffffff",borderColor:"#111111"});
  useEffect(()=>{if(!booth.photos.length)navigate("/session",{replace:true});},[booth.photos.length,navigate]);
  async function accept(){setWorking(true);const blob=await generateStrip(booth.photos,options);booth.setStripUrl(URL.createObjectURL(blob));setWorking(false);navigate("/result");}
  return <main className="booth-review"><header><span>REVIEW YOUR MOMENT</span><h1>Looking good?</h1><p>Keep the sequence or step back in for another take.</p></header>
    <div className={`review-grid ${options.layout}`}>{booth.photos.map((photo,index)=><figure key={photo.id}><img src={photo.previewUrl} alt={`Photo ${index+1}`}/><figcaption>{index+1}</figcaption></figure>)}</div>
    <section className="strip-options"><label>Layout<select value={options.layout} onChange={e=>setOptions({...options,layout:e.target.value as "vertical"|"horizontal"})}><option value="vertical">Vertical</option><option value="horizontal">Horizontal</option></select></label>
      <label>Spacing<input type="range" min="0" max="60" value={options.spacing} onChange={e=>setOptions({...options,spacing:+e.target.value})}/></label>
      <label>Background<input aria-label="Strip background" type="color" value={options.background} onChange={e=>setOptions({...options,background:e.target.value})}/></label></section>
    <div className="review-actions"><button onClick={()=>{booth.clearPhotos();navigate("/session")}}>Retake</button><button className="accept" onClick={accept} disabled={working}>{working?"Building strip…":"Accept photos"} <span>→</span></button></div>
  </main>;
}
