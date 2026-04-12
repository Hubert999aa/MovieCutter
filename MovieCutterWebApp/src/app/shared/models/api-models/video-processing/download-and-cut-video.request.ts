import { VideoPice } from "./video-cuttting-into-pices.request";

export interface DownloadAndCutVideoRequest {
  url: string;
  cutVideoInOnePiece: boolean;
  videoPices: VideoPice[];
}
