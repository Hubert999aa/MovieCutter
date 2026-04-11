import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { SourceApi } from './source.api';
import { profileSourceMap } from '@shared/static-data/source-type-mapping';
import { ProfileSource } from '@shared/models/profile-source';
import { Video } from '@shared/models/video';
import { CreateSourceRequest } from '@shared/models/api-models/source/create-source.request';
import { CreateSourceResponse } from '@shared/models/api-models/source/create-source.response';
import { UpdateSourceRequest } from '@shared/models/api-models/source/update-source.request';

@Injectable({ providedIn: 'root' })
export class SourceService {
  private readonly sourceApi = inject(SourceApi);

  loadSources(idProfile: number): Observable<ProfileSource[]> {
    return this.sourceApi.getSourceList(idProfile).pipe(
      map(items =>
        items.map(item => ({
          id: item.idSource,
          type: profileSourceMap[item.sourceType],
          name: item.name,
          baseUrl: item.baseUrl,
        }))
      )
    );
  }

  loadLastVideos(idSource: number): Observable<Video[]> {
    return this.sourceApi.getLastVideosList(idSource).pipe(
      map(items =>
        items.map(item => ({
          idVideo: item.id,
          title: item.title,
          url: item.url,
        }))
      )
    );
  }

  createSource(request: CreateSourceRequest): Observable<CreateSourceResponse> {
    return this.sourceApi.createSource(request);
  }

  updateSource(request: UpdateSourceRequest): Observable<Object> {
    return this.sourceApi.updateSource(request);
  }

  deleteSource(idSource: number): Observable<Object> {
    return this.sourceApi.deleteSource(idSource);
  }
}
