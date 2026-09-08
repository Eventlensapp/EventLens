import { useRef } from "react";
import { useEditorStore } from "./useEditorStore";
import { useBoothStore } from "../../../booth/store/useBoothStore";

export function Toolbar(){
  const add=useEditorStore(s=>s.add),undo=useEditorStore(s=>s.undo);const input=useRef<HTMLInputElement>(null);
  const boothPhotos=useBoothStore(s=>s.photos);
  const addPhoto=(source:string)=>add({id:crypto.randomUUID(),type:"photo",name:"Photo",source,x:110,y:150,width:980,height:1250,rotation:0,opacity:1,visible:true,locked:false,radius:18});
  return <div className="processing-toolbar"><button onClick={()=>boothPhotos[0]?addPhoto(boothPhotos[0].previewUrl):input.current?.click()}>＋ Photo</button>
    <button onClick={()=>add({id:crypto.randomUUID(),type:"text",name:"Custom text",text:"Your text",x:200,y:200,width:800,height:100,rotation:0,opacity:1,visible:true,locked:false,fontSize:56,align:"center",color:"#111111"})}>T Text</button>
    <button onClick={()=>add({id:crypto.randomUUID(),type:"shape",name:"Shape",x:250,y:250,width:400,height:240,rotation:0,opacity:1,visible:true,locked:false,fill:"#9b87f5",radius:24})}>□ Shape</button>
    <button onClick={undo}>↶ Undo</button><input ref={input} type="file" accept="image/jpeg,image/png" hidden onChange={e=>{const file=e.target.files?.[0];if(file&&file.size<=25*1024*1024)addPhoto(URL.createObjectURL(file));}}/></div>;
}
