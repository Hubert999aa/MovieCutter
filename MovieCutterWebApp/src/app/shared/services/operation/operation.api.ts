import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpWrapper } from '@core/providers/http-wrapper';
import { BaseOperationState } from '@shared/models/api-models/operation/base-operation-state';

@Injectable({ providedIn: 'root' })
export class OperationApi {
  private readonly httpWrapper = inject(HttpWrapper);
  private readonly operationBaseUrl = '/api/operation';

  getOperationStatuses(): Observable<BaseOperationState[]> {
    return this.httpWrapper.get<BaseOperationState[]>(`${this.operationBaseUrl}/getOperationStatuses`);
  }
}
