import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { VideoProcessingApi } from '@shared/services/video-processing/video-processing.api';
import { VideoCuttingIntoPicesRequest } from '@shared/models/api-models/video-processing/video-cuttting-into-pices.request';

@Injectable({ providedIn: 'root' })
export class VideoProcessingService {
  private readonly videoProcessingApi = inject(VideoProcessingApi);

  requestVideoDownload(videoUrl: string): Observable<Object> {
    return this.videoProcessingApi.requestVideoDownload(videoUrl);
  }

  requestVideoCuttingIntoPices(request: VideoCuttingIntoPicesRequest): Observable<Object> {
    return this.videoProcessingApi.requestVideoCuttingIntoPices(request);
  }
  
  requestVideoCuttingIntoFrames(sourceVideoFullPath: string): Observable<Object> {
    return this.videoProcessingApi.requestVideoCuttingIntoFrames(sourceVideoFullPath);
  }
}