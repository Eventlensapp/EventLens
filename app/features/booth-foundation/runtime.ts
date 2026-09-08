import type{Capability,Device,Permission,RuntimeReport}from"./types";
const testStorage=(storage:Storage)=>{try{const key="eventlens_booth_probe";storage.setItem(key,"1");storage.removeItem(key);return true}catch{return false}};
export async function detectBoothRuntime():Promise<RuntimeReport>{
 const n=navigator as Navigator&{usb?:unknown;bluetooth?:unknown;deviceMemory?:number;wakeLock?:unknown};
 const w=window as Window&{showOpenFilePicker?:unknown;OffscreenCanvas?:unknown};
 const capabilities:Capability[]=[
  ["Media devices",!!n.mediaDevices,"Device discovery foundation"],["Camera API",!!n.mediaDevices?.getUserMedia,"Permission capability only; no capture is started"],
  ["Device enumeration",!!n.mediaDevices?.enumerateDevices,"Discovers device metadata without opening media"],["Canvas",!!document.createElement("canvas").getContext,"2D rendering"],
  ["Offscreen canvas","OffscreenCanvas"in w,"Worker rendering"],["Web workers","Worker"in window,"Background processing"],["WebAssembly","WebAssembly"in window,"Portable runtime"],
  ["File system access","showOpenFilePicker"in w,"Browser-managed file access"],["IndexedDB","indexedDB"in window,"Offline foundation"],["Local storage",testStorage(localStorage),"Local configuration cache"],
  ["Session storage",testStorage(sessionStorage),"Temporary state"],["Clipboard",!!n.clipboard,"Clipboard API"],["Fullscreen",!!document.fullscreenEnabled,"Kiosk display"],
  ["Wake lock","wakeLock"in n,"Keep-awake support"],["Screen orientation","orientation"in screen,"Orientation API"],["Web Share",!!n.share,"Native sharing"],
  ["Notifications","Notification"in window,"Notification permission"],
 ].map(([name,supported,detail])=>({name:String(name),supported:Boolean(supported),detail:String(detail)}));
 let media:MediaDeviceInfo[]=[];try{media=n.mediaDevices?.enumerateDevices?await n.mediaDevices.enumerateDevices():[]}catch{}
 const devices:Device[]=media.map((d,index)=>({id:d.deviceId?`${d.kind}-${index}`:`device-${index}`,kind:d.kind,label:d.label||`${d.kind.replace("input"," input").replace("output"," output")} ${index+1}`,detail:"Browser-provided metadata"}));
 devices.push({id:"display",kind:"display",label:`${screen.width} × ${screen.height}`,detail:`${screen.orientation?.type||"unknown"} · ${devicePixelRatio}x pixel ratio`},{id:"input",kind:"input",label:matchMedia("(pointer: coarse)").matches?"Touch device":"Pointer device",detail:n.maxTouchPoints?`${n.maxTouchPoints} touch points`:"No touch points"},{id:"network",kind:"network",label:n.onLine?"Online":"Offline",detail:"Browser network state"},{id:"usb",kind:"foundation",label:"USB API",detail:"usb"in n?"Supported":"Unavailable"},{id:"bluetooth",kind:"foundation",label:"Bluetooth API",detail:"bluetooth"in n?"Supported":"Unavailable"});
 const permissionNames=["camera","microphone","notifications"] as const;const permissions:Permission[]=[];
 for(const permission of permissionNames){let state:Permission["state"]="Unknown";try{if(permission==="notifications")state=Notification.permission==="default"?"Prompt":capitalize(Notification.permission) as Permission["state"];else if(n.permissions)state=capitalize((await n.permissions.query({name:permission as PermissionName})).state)as Permission["state"]}catch{}permissions.push({permission,state,message:message(permission,state),canRetry:state!=="Granted"});}
 permissions.push({permission:"file-system",state:"Unknown",message:"The browser asks only when a file action is initiated.",canRetry:true});
 return{capabilities,devices,permissions,online:n.onLine,indexedDb:"indexedDB"in window,localStorage:testStorage(localStorage),memory:n.deviceMemory};
}
const capitalize=(value:string)=>value.charAt(0).toUpperCase()+value.slice(1);
const message=(name:string,state:Permission["state"])=>state==="Denied"?`${name} access is blocked in browser settings.`:state==="Granted"?`${name} access is available.`:`${name} permission will be requested only when needed.`;
export async function initializeBoothCache(){return new Promise<boolean>(resolve=>{if(!("indexedDB"in window))return resolve(false);const request=indexedDB.open("eventlens-booth-foundation",1);request.onupgradeneeded=()=>request.result.createObjectStore("configuration");request.onsuccess=()=>{request.result.close();resolve(true)};request.onerror=()=>resolve(false)});}
