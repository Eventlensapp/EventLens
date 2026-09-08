import type { ProcessingDocument } from "../canvas/types";

export const templateExamples:Record<string,ProcessingDocument>={
  wedding:{width:1200,height:1800,background:"#f8f1e8",layers:[
    {id:"bg-accent",type:"shape",name:"Warm border",x:34,y:34,width:1132,height:1732,rotation:0,opacity:1,visible:true,locked:true,fill:"#fffaf5",radius:30},
    {id:"title",type:"text",name:"Couple names",x:150,y:1500,width:900,height:100,rotation:0,opacity:1,visible:true,locked:false,text:"Rahul & Priya",fontFamily:"Georgia",fontSize:64,align:"center",color:"#7b4c3a"},
    {id:"date",type:"text",name:"Event date",x:250,y:1600,width:700,height:60,rotation:0,opacity:1,visible:true,locked:false,text:"28 JULY 2026",fontSize:26,align:"center",color:"#9b7868"}
  ]},
  corporate:{width:1200,height:1800,background:"#081528",layers:[
    {id:"brand",type:"shape",name:"Brand panel",x:0,y:1460,width:1200,height:340,rotation:0,opacity:1,visible:true,locked:true,fill:"#143b72"},
    {id:"message",type:"text",name:"Marketing message",x:90,y:1510,width:1020,height:110,rotation:0,opacity:1,visible:true,locked:false,text:"BUILDING WHAT'S NEXT",fontSize:52,align:"center",color:"#ffffff"},
    {id:"company",type:"text",name:"Company name",x:250,y:1640,width:700,height:60,rotation:0,opacity:1,visible:true,locked:false,text:"COMPANY EVENT",fontSize:25,align:"center",color:"#9bc4ff"}
  ]},
  retro:{width:1200,height:1800,background:"#f2cb78",layers:[
    {id:"retro-title",type:"text",name:"Title",x:100,y:1500,width:1000,height:100,rotation:-2,opacity:1,visible:true,locked:false,text:"GOOD TIMES!",fontFamily:"Georgia",fontSize:70,align:"center",color:"#b13f34",stroke:"#fff4cc",strokeWidth:5}
  ]}
};
