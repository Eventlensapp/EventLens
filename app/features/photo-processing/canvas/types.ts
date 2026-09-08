export type LayerType = "photo"|"shape"|"text"|"sticker"|"logo"|"frame";
export type CanvasLayer = {
  id:string; type:LayerType; name:string; x:number; y:number; width:number; height:number;
  rotation:number; opacity:number; visible:boolean; locked:boolean;
  source?:string; fill?:string; radius?:number; text?:string; fontFamily?:string;
  fontSize?:number; align?:"left"|"center"|"right"; color?:string;
  shadow?:boolean; stroke?:string; strokeWidth?:number;
};
export type ProcessingDocument = {
  width:number; height:number; background:string; layers:CanvasLayer[];
};
