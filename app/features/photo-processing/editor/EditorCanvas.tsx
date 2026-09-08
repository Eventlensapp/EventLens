"use client";
import { useEffect,useMemo,useRef,type PointerEvent as ReactPointerEvent } from "react";
import { CanvasRenderer } from "../canvas/CanvasRenderer";
import { useEditorStore } from "./useEditorStore";

export function EditorCanvas(){
  const canvasRef=useRef<HTMLCanvasElement>(null);const drag=useRef<{id:string;dx:number;dy:number}|null>(null);
  const renderer=useMemo(()=>new CanvasRenderer(),[]);const document=useEditorStore(s=>s.document);
  const selectedId=useEditorStore(s=>s.selectedId);const select=useEditorStore(s=>s.select);const update=useEditorStore(s=>s.update);
  useEffect(()=>{if(canvasRef.current)void renderer.render(canvasRef.current,document,.45);},[document,renderer]);
  const point=(event:ReactPointerEvent)=>{const rect=canvasRef.current!.getBoundingClientRect();return{x:(event.clientX-rect.left)/rect.width*document.width,y:(event.clientY-rect.top)/rect.height*document.height};};
  const down=(event:ReactPointerEvent)=>{const p=point(event);const layer=[...document.layers].reverse().find(l=>l.visible&&!l.locked&&p.x>=l.x&&p.x<=l.x+l.width&&p.y>=l.y&&p.y<=l.y+l.height);
    select(layer?.id??null);if(layer){drag.current={id:layer.id,dx:p.x-layer.x,dy:p.y-layer.y};event.currentTarget.setPointerCapture(event.pointerId);}};
  const move=(event:ReactPointerEvent)=>{if(!drag.current)return;const p=point(event);update(drag.current.id,{x:Math.round(p.x-drag.current.dx),y:Math.round(p.y-drag.current.dy)});};
  const selected=document.layers.find(l=>l.id===selectedId);
  return <div className="processing-stage"><div className="canvas-wrap"><canvas ref={canvasRef} onPointerDown={down} onPointerMove={move} onPointerUp={()=>drag.current=null} aria-label="Photo composition canvas"/>
    {selected&&<div className="selection-box" style={{left:`${selected.x/document.width*100}%`,top:`${selected.y/document.height*100}%`,width:`${selected.width/document.width*100}%`,height:`${selected.height/document.height*100}%`}}/>}</div>
    <small>Drag unlocked layers directly on the canvas</small></div>;
}
