import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpWrapper } from '@core/providers/http-wrapper';

@Injectable({ providedIn: 'root' })
export class VideoProcessingApi {
  private readonly httpWrapper = inject(HttpWrapper);
  private readonly videoProcessingBaseUrl = '/api/videoProcessing';

  requestVideoDownload(videoUrl: string): Observable<Object> {
    return this.httpWrapper.post<Object>(`${this.videoProcessingBaseUrl}/runVideoDownloading`, { Url: videoUrl });
  }
}