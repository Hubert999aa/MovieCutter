import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { VideoProcessingApi } from '@shared/services/video-processing/video-processing.api';
import { VideoCuttingIntoPicesRequest } from '@shared/models/api-models/video-processing/video-cuttting-into-pices.request';
import { DownloadAndCutVideoRequest } from '@shared/models/api-models/video-processing/download-and-cut-video.request';

@Injectable({ providedIn: 'root' })
export class VideoProcessingService {
  private readonly videoProcessingApi = inject(VideoProcessingApi);

  requestVideoDownload(videoUrl: string, videoName: string): Observable<Object> {
    return this.videoProcessingApi.requestVideoDownload(videoUrl, videoName);
  }

  requestVideoCuttingIntoPices(request: VideoCuttingIntoPicesRequest): Observable<Object> {
    return this.videoProcessingApi.requestVideoCuttingIntoPices(request);
  }
  
  requestVideoCuttingIntoFrames(sourceVideoFullPath: string): Observable<Object> {
    return this.videoProcessingApi.requestVideoCuttingIntoFrames(sourceVideoFullPath);
  }

  requestDownloadAndCutVideo(request: DownloadAndCutVideoRequest): Observable<Object> {
    return this.videoProcessingApi.requestDownloadAndCutVideo(request);
  }
}