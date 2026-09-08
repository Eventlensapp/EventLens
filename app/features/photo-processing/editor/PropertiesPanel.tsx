import { stickerLibrary } from "../templates/stickers";
import { useEditorStore } from "./useEditorStore";
export function PropertiesPanel(){
  const {document,selectedId,update,add}=useEditorStore();const layer=document.layers.find(l=>l.id===selectedId);
  return <aside className="properties-panel"><h2>Properties</h2>{layer?<div className="property-fields">
    {layer.type==="text"&&<><label>Text<textarea value={layer.text} onChange={e=>update(layer.id,{text:e.target.value})}/></label><label>Font<select value={layer.fontFamily} onChange={e=>update(layer.id,{fontFamily:e.target.value})}><option>Manrope</option><option>Georgia</option><option>Arial</option><option>Courier New</option></select></label><label>Size<input type="number" value={layer.fontSize} onChange={e=>update(layer.id,{fontSize:+e.target.value})}/></label><label>Color<input type="color" value={layer.color} onChange={e=>update(layer.id,{color:e.target.value})}/></label></>}
    <label>Rotation<input type="range" min="-180" max="180" value={layer.rotation} onChange={e=>update(layer.id,{rotation:+e.target.value})}/></label>
    <label>Width<input type="number" value={layer.width} onChange={e=>update(layer.id,{width:Math.max(20,+e.target.value)})}/></label>
    <label>Height<input type="number" value={layer.height} onChange={e=>update(layer.id,{height:Math.max(20,+e.target.value)})}/></label>
    <label>Opacity<input type="range" min="0" max="1" step=".05" value={layer.opacity} onChange={e=>update(layer.id,{opacity:+e.target.value})}/></label>
    <label><input type="checkbox" checked={layer.shadow??false} onChange={e=>update(layer.id,{shadow:e.target.checked})}/> Shadow</label>
  </div>:<div className="sticker-library"><p>Select a layer or add a sticker.</p>{Object.entries(stickerLibrary).map(([category,items])=><section key={category}><span>{category}</span><div>{items.map(sticker=><button key={sticker} onClick={()=>add({id:crypto.randomUUID(),type:"sticker",name:`${category} sticker`,text:sticker,x:450,y:400,width:300,height:180,rotation:0,opacity:1,visible:true,locked:false,fontSize:110,align:"center"})}>{sticker}</button>)}</div></section>)}</div>}</aside>;
}
