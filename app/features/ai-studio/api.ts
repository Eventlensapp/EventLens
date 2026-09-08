import type { AIJob } from "./types";
import {apiRequest} from "../../lib/api";
export const queueAI=(action:string,payload:object)=>apiRequest<AIJob>(`/api/ai/${action}`,{method:"POST",body:JSON.stringify(payload)});
export const getAIJobs=(eventId?:string)=>apiRequest<AIJob[]>(`/api/ai/jobs${eventId?`?eventId=${eventId}`:""}`);
