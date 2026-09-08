import { create } from "zustand";
import type { CanvasLayer, ProcessingDocument } from "../canvas/types";
import { templateExamples } from "../templates/examples";

type EditorState={document:ProcessingDocument;selectedId:string|null;history:ProcessingDocument[];
  select:(id:string|null)=>void;add:(layer:CanvasLayer)=>void;update:(id:string,patch:Partial<CanvasLayer>)=>void;
  remove:(id:string)=>void;duplicate:(id:string)=>void;move:(id:string,direction:-1|1)=>void;load:(document:ProcessingDocument)=>void;undo:()=>void;};
const clone=(value:ProcessingDocument)=>structuredClone(value);
export const useEditorStore=create<EditorState>((set,get)=>({
  document:clone(templateExamples.wedding),selectedId:null,history:[],
  select:selectedId=>set({selectedId}),
  add:layer=>set(s=>({history:[...s.history.slice(-19),clone(s.document)],document:{...s.document,layers:[...s.document.layers,layer]},selectedId:layer.id})),
  update:(id,patch)=>set(s=>({history:[...s.history.slice(-19),clone(s.document)],document:{...s.document,layers:s.document.layers.map(l=>l.id===id?{...l,...patch}:l)}})),
  remove:id=>set(s=>({history:[...s.history.slice(-19),clone(s.document)],document:{...s.document,layers:s.document.layers.filter(l=>l.id!==id)},selectedId:null})),
  duplicate:id=>{const source=get().document.layers.find(l=>l.id===id);if(source)get().add({...source,id:crypto.randomUUID(),name:`${source.name} copy`,x:source.x+24,y:source.y+24});},
  move:(id,direction)=>set(s=>{const layers=[...s.document.layers],i=layers.findIndex(l=>l.id===id),target=Math.max(0,Math.min(layers.length-1,i+direction));[layers[i],layers[target]]=[layers[target],layers[i]];return{history:[...s.history.slice(-19),clone(s.document)],document:{...s.document,layers}};}),
  load:document=>set(s=>({history:[...s.history.slice(-19),clone(s.document)],document:clone(document),selectedId:null})),
  undo:()=>set(s=>{const previous=s.history.at(-1);return previous?{document:previous,history:s.history.slice(0,-1),selectedId:null}:s;})
}));
