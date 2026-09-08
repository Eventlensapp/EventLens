import type { CanvasLayer, ProcessingDocument } from "./types";

const imageCache = new Map<string,Promise<HTMLImageElement>>();
const loadImage=(source:string)=>{if(!imageCache.has(source))imageCache.set(source,new Promise((resolve,reject)=>{const image=new Image();image.onload=()=>resolve(image);image.onerror=reject;image.src=source;}));return imageCache.get(source)!;};

export class CanvasRenderer {
  async render(canvas:HTMLCanvasElement,document:ProcessingDocument,scale=1){
    canvas.width=Math.round(document.width*scale);canvas.height=Math.round(document.height*scale);
    const context=canvas.getContext("2d",{alpha:false});if(!context)throw new Error("Canvas unavailable.");
    context.scale(scale,scale);context.fillStyle=document.background;context.fillRect(0,0,document.width,document.height);
    for(const layer of document.layers)if(layer.visible)await this.drawLayer(context,layer);
  }
  private async drawLayer(context:CanvasRenderingContext2D,layer:CanvasLayer){
    context.save();context.globalAlpha=layer.opacity;context.translate(layer.x+layer.width/2,layer.y+layer.height/2);context.rotate(layer.rotation*Math.PI/180);
    const x=-layer.width/2,y=-layer.height/2;
    if(layer.shadow){context.shadowColor="#0008";context.shadowBlur=24;context.shadowOffsetY=10;}
    if(layer.type==="photo"||layer.type==="logo"||layer.type==="frame"){
      if(layer.source){const image=await loadImage(layer.source);context.beginPath();context.roundRect(x,y,layer.width,layer.height,layer.radius??0);context.clip();
        const ratio=Math.max(layer.width/image.width,layer.height/image.height);const w=image.width*ratio,h=image.height*ratio;context.drawImage(image,-w/2,-h/2,w,h);}
    }else if(layer.type==="shape"){context.fillStyle=layer.fill??"#ffffff";context.beginPath();context.roundRect(x,y,layer.width,layer.height,layer.radius??0);context.fill();}
    else {context.textAlign=layer.align??"center";context.textBaseline="middle";context.font=`700 ${layer.fontSize??48}px ${layer.fontFamily??"Manrope"}`;
      context.fillStyle=layer.color??"#111111";if(layer.stroke&&layer.strokeWidth){context.strokeStyle=layer.stroke;context.lineWidth=layer.strokeWidth;context.strokeText(layer.text??"",0,0,layer.width);}
      context.fillText(layer.text??"",0,0,layer.width);}
    context.restore();
  }
  async export(model:ProcessingDocument,format:"png"|"jpeg",quality=.94){
    const canvas=globalThis.document.createElement("canvas");
    await this.render(canvas,model);
    return new Promise<Blob>((resolve,reject)=>canvas.toBlob(blob=>blob?resolve(blob):reject(new Error("Export failed.")),`image/${format}`,quality));
  }
}
