"use client";
import { useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { CanvasRenderer } from "../canvas/CanvasRenderer";
import { templateExamples } from "../templates/examples";
import { EditorCanvas } from "./EditorCanvas";
import { LayersPanel } from "./LayersPanel";
import { PropertiesPanel } from "./PropertiesPanel";
import { Toolbar } from "./Toolbar";
import { useEditorStore } from "./useEditorStore";

export default function PhotoProcessingEditor(){
  const navigate=useNavigate(),model=useEditorStore(s=>s.document),load=useEditorStore(s=>s.load),renderer=useMemo(()=>new CanvasRenderer(),[]);
  const exportImage=async(format:"png"|"jpeg")=>{const blob=await renderer.export(model,format),url=URL.createObjectURL(blob),a=document.createElement("a");a.href=url;a.download=`eventlens-design.${format==="jpeg"?"jpg":"png"}`;a.click();setTimeout(()=>URL.revokeObjectURL(url),1000);};
  return <main className="processing-editor"><header><button onClick={()=>navigate(-1)}>← Back</button><div><span>PHOTO PROCESSING</span><h1>Design studio</h1></div><nav><button onClick={()=>exportImage("png")}>PNG</button><button onClick={()=>exportImage("jpeg")}>JPEG</button><button onClick={()=>window.print()}>PDF</button></nav></header>
    <div className="template-ribbon"><span>Templates</span>{Object.entries(templateExamples).map(([name,template])=><button onClick={()=>load(template)} key={name}>{name}</button>)}</div>
    <Toolbar/><div className="editor-workspace"><LayersPanel/><EditorCanvas/><PropertiesPanel/></div></main>;
}
