import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpWrapper } from '@core/providers/http-wrapper';
import { SourceListItem } from '@shared/models/api-models/source/source-list-item';
import { WebVideoItem } from '@src/app/shared/models/api-models/source/web-video-item';
import { CreateSourceRequest } from '@shared/models/api-models/source/create-source.request';
import { CreateSourceResponse } from '@shared/models/api-models/source/create-source.response';
import { UpdateSourceRequest } from '@shared/models/api-models/source/update-source.request';

@Injectable({ providedIn: 'root' })
export class SourceApi {
  private readonly httpWrapper = inject(HttpWrapper);
  private readonly sourceBaseUrl = '/api/source';

  getSourceList(idProfile: number): Observable<SourceListItem[]> {
    return this.httpWrapper.get<SourceListItem[]>(`${this.sourceBaseUrl}/getSourceList?id=${idProfile}`);
  }

  getLastVideosList(idSource: number): Observable<WebVideoItem[]> {
    return this.httpWrapper.get<WebVideoItem[]>(`${this.sourceBaseUrl}/getLastVideosList?id=${idSource}`);
  }

  createSource(request: CreateSourceRequest): Observable<CreateSourceResponse> {
    return this.httpWrapper.post<CreateSourceResponse>(`${this.sourceBaseUrl}/createSource`, request);
  }

  updateSource(request: UpdateSourceRequest): Observable<Object> {
    return this.httpWrapper.put<Object>(`${this.sourceBaseUrl}/updateSource`, request);
  }

  deleteSource(idSource: number): Observable<Object> {
    return this.httpWrapper.delete<Object>(`${this.sourceBaseUrl}/deleteSource`, { idSource: idSource });
  }
}
