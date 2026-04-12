import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpWrapper } from '@core/providers/http-wrapper';
import { VideoCuttingIntoPicesRequest } from '@shared/models/api-models/video-processing/video-cuttting-into-pices.request';

@Injectable({ providedIn: 'root' })
export class VideoProcessingApi {
  private readonly httpWrapper = inject(HttpWrapper);
  private readonly videoProcessingBaseUrl = '/api/videoProcessing';

  requestVideoDownload(videoUrl: string): Observable<Object> {
    return this.httpWrapper.post<Object>(`${this.videoProcessingBaseUrl}/runVideoDownloading`, { Url: videoUrl });
  }

  requestVideoCuttingIntoPices(request: VideoCuttingIntoPicesRequest): Observable<Object> {
    return this.httpWrapper.post<Object>(`${this.videoProcessingBaseUrl}/runVideoCuttingIntoPices`, request);
  }

  requestVideoCuttingIntoFrames(sourceVideoFullPath: string): Observable<Object> {
    return this.httpWrapper.post<Object>(`${this.videoProcessingBaseUrl}/runVideoCuttingIntoFrames`, { SourceVideoFullPath: sourceVideoFullPath });
  }
}