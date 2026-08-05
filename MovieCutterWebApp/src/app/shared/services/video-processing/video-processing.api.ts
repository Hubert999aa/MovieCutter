import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpWrapper } from '@core/providers/http-wrapper';
import { VideoCuttingIntoPicesRequest } from '@shared/models/api-models/video-processing/video-cuttting-into-pices.request';
import { DownloadAndCutVideoRequest } from '@shared/models/api-models/video-processing/download-and-cut-video.request';

@Injectable({ providedIn: 'root' })
export class VideoProcessingApi {
  private readonly httpWrapper = inject(HttpWrapper);
  private readonly videoProcessingBaseUrl = '/api/videoProcessing';

  requestVideoDownload(videoUrl: string, videoName: string): Observable<Object> {
    return this.httpWrapper.post<Object>(`${this.videoProcessingBaseUrl}/runVideoDownloading`, { Url: videoUrl, VideoName: videoName });
  }

  requestVideoCuttingIntoPices(request: VideoCuttingIntoPicesRequest): Observable<Object> {
    return this.httpWrapper.post<Object>(`${this.videoProcessingBaseUrl}/runVideoCuttingIntoPices`, request);
  }

  requestVideoCuttingIntoFrames(sourceVideoFullPath: string): Observable<Object> {
    return this.httpWrapper.post<Object>(`${this.videoProcessingBaseUrl}/runVideoCuttingIntoFrames`, { SourceVideoFullPath: sourceVideoFullPath });
  }

  requestDownloadAndCutVideo(request: DownloadAndCutVideoRequest): Observable<Object> {
    return this.httpWrapper.post<Object>(`${this.videoProcessingBaseUrl}/runDownloadAndCutVideo`, request);
  }
}