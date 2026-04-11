import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpWrapper } from '@core/providers/http-wrapper';
import { FileMetadata } from '@shared/models/file-metadata';

@Injectable({ providedIn: 'root' })
export class FileApi {
  private readonly httpWrapper = inject(HttpWrapper);
  private readonly filesBaseUrl = '/api/files';

  getFilesMetadata(): Observable<FileMetadata[]> {
    return this.httpWrapper.get<FileMetadata[]>(`${this.filesBaseUrl}/getFilesList`);
  }
}
