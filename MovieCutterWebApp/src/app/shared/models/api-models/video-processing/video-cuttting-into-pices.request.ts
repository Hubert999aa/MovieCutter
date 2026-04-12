export interface VideoCuttingIntoPicesRequest {
  sourceVideoFullPath: string;
  videoPices: VideoPice[];
}

export interface VideoPice {
  startTime: string;
  endTime: string;
}