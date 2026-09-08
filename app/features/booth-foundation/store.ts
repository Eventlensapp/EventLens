import{create}from"zustand";import type{RuntimeReport}from"./types";
type State={step:number;report:RuntimeReport|null;cacheReady:boolean;setStep:(step:number)=>void;setReport:(report:RuntimeReport)=>void;setCacheReady:(ready:boolean)=>void};
export const useBoothFoundationStore=create<State>(set=>({step:0,report:null,cacheReady:false,setStep:step=>set({step}),setReport:report=>set({report}),setCacheReady:cacheReady=>set({cacheReady})}));
