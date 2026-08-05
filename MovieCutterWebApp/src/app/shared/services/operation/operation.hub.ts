import { Injectable, inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';

import { AppConfigurationLoader } from '@core/providers/app-configuration-loader';
import { ErrorHandler } from '@core/providers/error-handler';
import { OperationStatus } from '@shared/enums/operation-status';
import { OperationProgressUpdate } from '@shared/models/api-models/operation/operation-progress-update';
import { OperationStatusUpdate } from '@shared/models/api-models/operation/operation-status-update';

const HUB_URL = '/hubs/operationsProgress';
const PROGRESS_MESSAGE = 'OperationProgress';
const STATUS_MESSAGE = 'OperationStatus';

// Payload property casing depends on the API serializer settings - both conventions are accepted.
function readValue(payload: unknown, camelCaseKey: string, pascalCaseKey: string): unknown {
  if (typeof payload !== 'object' || payload === null) return undefined;
  const record = payload as Record<string, unknown>;
  return record[camelCaseKey] ?? record[pascalCaseKey];
}

@Injectable({ providedIn: 'root' })
export class OperationHub {
  private readonly appConfigurationLoader = inject(AppConfigurationLoader);
  private readonly errorHandler = inject(ErrorHandler);

  private connection: HubConnection | null = null;

  private readonly progressUpdates = new Subject<OperationProgressUpdate>();
  private readonly statusUpdates = new Subject<OperationStatusUpdate>();
  private readonly reconnected = new Subject<void>();

  readonly progressUpdates$: Observable<OperationProgressUpdate> = this.progressUpdates.asObservable();
  readonly statusUpdates$: Observable<OperationStatusUpdate> = this.statusUpdates.asObservable();
  readonly reconnected$: Observable<void> = this.reconnected.asObservable();

  connect(): void {
    if (this.connection) return;

    const connection = new HubConnectionBuilder()
      .withUrl(this.appConfigurationLoader.apiBaseUrl + HUB_URL)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Trace)
      .build();

    connection.on(PROGRESS_MESSAGE, (payload: unknown) => {
      console.log(payload);
      const update = this.toProgressUpdate(payload);
      if (update) this.progressUpdates.next(update);
    });

    connection.on(STATUS_MESSAGE, (payload: unknown) => {
      console.log(payload);
      const update = this.toStatusUpdate(payload);
      if (update) this.statusUpdates.next(update);
    });

    connection.onreconnected(() => this.reconnected.next());

    this.connection = connection;

    connection.start().catch(err => console.log("DUPAAAAA" + err));
  }

  disconnect(): void {
    const connection = this.connection;
    if (!connection) return;

    this.connection = null;

    if (connection.state !== HubConnectionState.Disconnected) {
      connection.stop().catch(err => this.errorHandler.logError(err));
    }
  }

  private toProgressUpdate(payload: unknown): OperationProgressUpdate | null {
    const idOperation = readValue(payload, 'idOperation', 'IdOperation');
    const progressPercent = readValue(payload, 'progressPercent', 'ProgressPercent');

    if (typeof idOperation !== 'number' || typeof progressPercent !== 'number') {
      this.errorHandler.logError(`Nieprawidłowa wiadomość ${PROGRESS_MESSAGE}: ${JSON.stringify(payload)}`);
      return null;
    }

    return { idOperation, progressPercent };
  }

  private toStatusUpdate(payload: unknown): OperationStatusUpdate | null {
    const idOperation = readValue(payload, 'idOperation', 'IdOperation');
    const operationStatus = readValue(payload, 'operationStatus', 'OperationStatus');

    if (typeof idOperation !== 'number' || typeof operationStatus !== 'string') {
      this.errorHandler.logError(`Nieprawidłowa wiadomość ${STATUS_MESSAGE}: ${JSON.stringify(payload)}`);
      return null;
    }

    return { idOperation, operationStatus: operationStatus as OperationStatus };
  }
}
