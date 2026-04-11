import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { VideoProcessingApi } from '@shared/services/video-processing/video-processing.api';


@Injectable({ providedIn: 'root' })
export class VideoProcessingService {
  private readonly videoProcessingApi = inject(VideoProcessingApi);

  requestVideoDownload(videoUrl: string): Observable<Object> {
    return this.videoProcessingApi.requestVideoDownload(videoUrl);
  }
}