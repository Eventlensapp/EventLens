import { useEditorStore } from "./useEditorStore";
export function LayersPanel(){
  const {document,selectedId,select,move,remove,duplicate}=useEditorStore();
  return <aside className="layers-panel"><h2>Layers</h2><div>{[...document.layers].reverse().map(layer=><button className={selectedId===layer.id?"active":""} onClick={()=>select(layer.id)} key={layer.id}><span>{layer.type==="text"?"T":layer.type==="photo"?"▧":"◆"}</span><b>{layer.name}</b><i>{layer.locked?"⌑":layer.visible?"◉":"○"}</i></button>)}</div>
    {selectedId&&<footer><button onClick={()=>move(selectedId,1)}>↑</button><button onClick={()=>move(selectedId,-1)}>↓</button><button onClick={()=>duplicate(selectedId)}>Duplicate</button><button onClick={()=>remove(selectedId)}>Delete</button></footer>}</aside>;
}
