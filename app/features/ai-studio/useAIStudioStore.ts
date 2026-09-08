import {create} from "zustand";
type Edit={style?:string;background?:string;props:string[];enhancements:string[];upscale:2|4|8;prompt:string};
export type AIStudioState={edit:Edit;past:Edit[];future:Edit[];compare:boolean;set:(patch:Partial<Edit>)=>void;undo:()=>void;redo:()=>void;toggleCompare:()=>void};
const initial:Edit={props:[],enhancements:[],upscale:2,prompt:""};
export const useAIStudioStore=create<AIStudioState>((set,get)=>({edit:initial,past:[],future:[],compare:false,
 set:patch=>set(s=>({past:[...s.past.slice(-19),s.edit],edit:{...s.edit,...patch},future:[]})),
 undo:()=>{const s=get(),previous=s.past.at(-1);if(previous)set({edit:previous,past:s.past.slice(0,-1),future:[s.edit,...s.future]});},
 redo:()=>{const s=get(),next=s.future[0];if(next)set({edit:next,past:[...s.past,s.edit],future:s.future.slice(1)});},
 toggleCompare:()=>set(s=>({compare:!s.compare}))}));
