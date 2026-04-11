import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { FileApi } from './files.api';
import { FileMetadata } from '@shared/models/file-metadata';

@Injectable({ providedIn: 'root' })
export class FilesService {
  private readonly fileApi = inject(FileApi);

  getFilesMetadata(): Observable<FileMetadata[]> {
    return this.fileApi.getFilesMetadata();
  }
}
