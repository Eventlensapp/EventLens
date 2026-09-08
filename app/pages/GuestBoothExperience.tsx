"use client";
import{useCallback,useEffect,useMemo,useRef,useState}from"react";
import{useQuery}from"@tanstack/react-query";
import{useSearchParams}from"react-router-dom";
import{BrowserCameraManager}from"../features/camera-foundation/CameraManager";
import{captureFrame,shutterSound,wait}from"../features/capture-engine/CaptureEngine";
import{localCaptureStorage}from"../features/capture-engine/IndexedDbCaptureStorage";
import{useSessionEngineStore}from"../features/session-engine/store";
import{guestBoothApi}from"../features/guest-booth/api";
import{savedGuestJourney,useGuestBoothStore}from"../features/guest-booth/guestBoothStore";
import type{GuestBoothState,GuestMode}from"../features/guest-booth/types";
import{AttractScreen,WelcomeScreen,ModeSelectionScreen,PreparationScreen,GuestCountdown,CaptureScreen,PreviewScreen,TemplateSelectionScreen,CompletionScreen,ErrorRecoveryScreen}from"../features/guest-booth/components/GuestScreens";
import"../guest-booth.css";

export default function GuestBoothExperience(){
 const[params]=useSearchParams();
 const managed=useSessionEngineStore(x=>x.session);
 const eventId=params.get("eventId")||managed?.eventId||"";
 const store=useGuestBoothStore();
 const manager=useMemo(()=>new BrowserCameraManager(),[]);
 const video=useRef<HTMLVideoElement|null>(null);
 const[count,setCount]=useState(3),[photo,setPhoto]=useState(1),[urls,setUrls]=useState<string[]>([]);
 const busy=useRef(false),lastActivity=useRef(Date.now());
 const experience=useQuery({queryKey:["guest-booth-experience",eventId],queryFn:()=>guestBoothApi.configuration(eventId),enabled:!!eventId,retry:1});
 const setJourney=store.setJourney;
 const move=useCallback(async(next:GuestBoothState,mode?:GuestMode,reason?:string)=>{
  if(!store.journey)return;
  try{setJourney(await guestBoothApi.transition(store.journey.sessionId,store.journey.recoveryToken,next,mode,reason))}
  catch(e){store.fail(e instanceof Error?e.message:"The experience paused unexpectedly.")}
 },[store.journey,setJourney,store]);

 useEffect(()=>{
  const saved=savedGuestJourney();
  if(saved&&saved.eventId===eventId)void guestBoothApi.recover(saved.sessionId,saved.recoveryToken).then(x=>x&&setJourney(x)).catch(()=>store.clear());
  return()=>manager.destroy();
 },[eventId,manager,setJourney]);

 useEffect(()=>{
  const touch=()=>lastActivity.current=Date.now();
  for(const x of["pointerdown","keydown","touchstart"]as const)addEventListener(x,touch,{passive:true});
  const timer=setInterval(()=>{
   if(store.state!=="Attract"&&experience.data&&Date.now()-lastActivity.current>experience.data.configuration.attractTimeout*1000){
    manager.stopPreview();urls.forEach(URL.revokeObjectURL);setUrls([]);store.clear();
   }
  },1000);
  const leave=(e:BeforeUnloadEvent)=>{if(store.state!=="Attract"&&store.state!=="Completed"){e.preventDefault();e.returnValue=""}};
  addEventListener("beforeunload",leave);
  return()=>{clearInterval(timer);removeEventListener("beforeunload",leave);for(const x of["pointerdown","keydown","touchstart"]as const)removeEventListener(x,touch)};
 },[store.state,experience.data,manager,urls,store]);

 useEffect(()=>{
  if(store.state!=="Countdown")return;
  let cancelled=false;
  void(async()=>{for(let n=3;n>=0;n--){if(cancelled)return;setCount(n);if(experience.data?.configuration.soundEnabled&&n>0)shutterSound();await wait(n?800:500)}if(!cancelled)await move("Capturing")})();
  return()=>{cancelled=true};
 },[store.state,move,experience.data]);

 useEffect(()=>{
  if(store.state!=="Capturing"||busy.current)return;
  busy.current=true;
  void(async()=>{
   try{
    if(!video.current)throw new Error("Camera display is unavailable.");
    await manager.startPreview(video.current);
    const total=store.mode==="Photo"?3:1,next:string[]=[];
    for(let n=1;n<=total;n++){
     setPhoto(n);await wait(700);
     const frame=await captureFrame(video.current,.9,true);shutterSound();
     const key=`guest:${store.journey!.sessionId}:${crypto.randomUUID()}`;
     await localCaptureStorage.put({key,sessionId:store.journey!.sessionId,captureNumber:n,blob:frame.blob,capturedAt:new Date().toISOString(),metadataSaved:true});
     next.push(URL.createObjectURL(frame.blob));
    }
    manager.stopPreview();setUrls(next);await move("Preview");
   }catch(e){manager.stopPreview();store.fail(e instanceof Error?friendly(e.message):"The camera needs a moment.")}
   finally{busy.current=false}
  })();
 },[store.state,store.mode,store.journey,manager,move,store]);

 const begin=async()=>{
  if(!eventId)return;
  try{
   const x=await guestBoothApi.start(eventId,managed?.sessionId);setJourney(x);
   if(experience.data?.configuration.fullscreenEnabled&&!document.fullscreenElement)await document.documentElement.requestFullscreen().catch(()=>{});
   await guestBoothApi.transition(x.sessionId,x.recoveryToken,"Welcome").then(setJourney);
  }catch(e){store.fail(e instanceof Error?e.message:"This booth is not ready.")}
 };
 const restart=()=>{manager.stopPreview();urls.forEach(URL.revokeObjectURL);setUrls([]);store.clear()};
 const retake=async()=>{urls.forEach(URL.revokeObjectURL);setUrls([]);await move("Preparation",store.mode,"Guest requested retake")};

 if(!eventId)return <main className="guest-unavailable"><div>✦</div><h1>Booth event required</h1><p>Open this kiosk from an active EventLens event or include its event link.</p></main>;
 if(experience.isLoading)return <main className="guest-loading"><div>✦</div><p>Preparing your experience…</p></main>;
 if(experience.isError||!experience.data)return <main className="guest-unavailable"><h1>This experience is taking a pause.</h1><button onClick={()=>experience.refetch()}>Try again</button></main>;
 const props={experience:experience.data};
 return <div className={`guest-booth theme-${experience.data.configuration.theme.toLowerCase()} animation-${experience.data.configuration.animationType.toLowerCase()}`}>
  {store.state==="Attract"&&<AttractScreen {...props}onStart={begin}/>}
  {store.state==="Welcome"&&<WelcomeScreen {...props}onStart={()=>move("ModeSelection")}onLanguage={()=>{}}/>}
  {store.state==="ModeSelection"&&<ModeSelectionScreen selected={store.mode}onSelect={store.setMode}onContinue={()=>move("Preparation",store.mode)}/>}
  {store.state==="Preparation"&&<PreparationScreen onReady={()=>move("Countdown")}/>}
  {store.state==="Countdown"&&<GuestCountdown count={count}/>}
  {store.state==="Capturing"&&<CaptureScreen video={x=>video.current=x}photo={photo}total={store.mode==="Photo"?3:1}/>}
  {store.state==="Preview"&&<PreviewScreen urls={urls}onRetake={retake}onContinue={()=>move("TemplateSelection")}onCancel={restart}/>}
  {store.state==="TemplateSelection"&&<TemplateSelectionScreen onChoose={async()=>{await move("Rendering");setTimeout(()=>void move("Completed"),900)}}/>}
  {store.state==="Rendering"&&<main className="guest-screen"><div className="guest-spinner"/><small>CREATING YOUR MOMENT</small><h1>Adding the finishing touch…</h1></main>}
  {store.state==="Completed"&&<CompletionScreen onFinish={async()=>{if(document.fullscreenElement)await document.exitFullscreen().catch(()=>{});restart()}}/>}
  {store.state==="Error"&&<ErrorRecoveryScreen message={store.error||"Please try once more."}onRetry={async()=>{if(store.journey)await move("Preparation",store.mode,"Guest recovery");else await begin()}}onRestart={restart}/>}
  <button className="guest-exit"aria-label="Exit fullscreen"onClick={()=>document.fullscreenElement&&document.exitFullscreen()}>×</button>
 </div>;
}
function friendly(message:string){return /permission|NotAllowed/i.test(message)?"Please allow camera access, then tap Try again.":/busy|NotReadable/i.test(message)?"The camera is busy. Close other camera apps and try again.":"We could not start the camera. Check that it is connected and try again."}
