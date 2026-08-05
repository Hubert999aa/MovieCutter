import { VideoPice } from "./video-cuttting-into-pices.request";

export interface DownloadAndCutVideoRequest {
  url: string;
  videoName: string;
  cutVideoInOnePiece: boolean;
  videoPices: VideoPice[];
}
