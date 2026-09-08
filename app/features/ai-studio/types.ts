export type AIJobStatus="Queued"|"Running"|"Completed"|"Failed"|"RetryScheduled";
export type AIJob={id:string;eventId:string;jobType:string;provider:string;status:AIJobStatus;progress:number;inputImage:string;outputImage?:string;errorMessage?:string;createdAt?:string};
export const styles=["Anime","Cartoon","Pixar Inspired","Comic","Vintage","Oil Painting","Sketch","Watercolor","Fantasy","Royal Portrait","Cyberpunk","Studio Portrait"] as const;
export const backgrounds=["Wedding","Birthday","Corporate","Nature","Beach","Mountains","Festival","Luxury","Minimal","Space","Christmas","Halloween","New Year"] as const;
export const props=["Hats","Crowns","Flowers","Glasses","Party accessories","Graduation caps","Christmas hats","Masks"] as const;
