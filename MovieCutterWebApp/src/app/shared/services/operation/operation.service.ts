import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { OperationApi } from './operation.api';
import { OperationHub } from './operation.hub';
import { BaseOperationState } from '@shared/models/api-models/operation/base-operation-state';
import { OperationProgressUpdate } from '@shared/models/api-models/operation/operation-progress-update';
import { OperationStatusUpdate } from '@shared/models/api-models/operation/operation-status-update';

@Injectable({ providedIn: 'root' })
export class OperationService {
  private readonly operationApi = inject(OperationApi);
  private readonly operationHub = inject(OperationHub);

  readonly progressUpdates$: Observable<OperationProgressUpdate> = this.operationHub.progressUpdates$;
  readonly statusUpdates$: Observable<OperationStatusUpdate> = this.operationHub.statusUpdates$;

  /** Emituje po odzyskaniu połączenia z hubem - stan trzeba wtedy pobrać ponownie z API. */
  readonly reconnected$: Observable<void> = this.operationHub.reconnected$;

  loadOperations(): Observable<BaseOperationState[]> {
    return this.operationApi.getOperationStatuses();
  }

  connect(): void {
    this.operationHub.connect();
  }

  disconnect(): void {
    this.operationHub.disconnect();
  }
}
