import { Component, ChangeDetectionStrategy, signal, computed, inject, OnInit, OnDestroy } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { OperationStatus } from '@shared/enums/operation-status';
import { OperationType } from '@shared/enums/operation-type';
import { BaseOperationState } from '@shared/models/api-models/operation/base-operation-state';
import { OperationProgressUpdate } from '@shared/models/api-models/operation/operation-progress-update';
import { OperationStatusUpdate } from '@shared/models/api-models/operation/operation-status-update';
import { OperationService } from '@shared/services/operation/operation.service';
import { ToastService } from '@shared/services/toast/toast.service';
import { LoaderComponent } from '@shared/components/loader/loader';

@Component({
  selector: 'app-task-queue',
  imports: [LoaderComponent],
  templateUrl: './task-queue.html',
  styleUrl: './task-queue.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskQueueComponent implements OnInit, OnDestroy {
  private readonly operationService = inject(OperationService);
  private readonly toast = inject(ToastService);

  readonly operations = signal<BaseOperationState[]>([]);
  readonly loading = signal(false);

  readonly activeCount = computed(
    () =>
      this.operations().filter(
        operation =>
          operation.operationStatus === OperationStatus.Queued ||
          operation.operationStatus === OperationStatus.Processing
      ).length
  );

  readonly typeConfig: Record<OperationType, { label: string; icon: string }> = {
    [OperationType.Undefined]:               { label: 'Operacja',            icon: 'pi pi-question-circle' },
    [OperationType.DownloadOnly]:            { label: 'Pobieranie',          icon: 'pi pi-download' },
    [OperationType.CuttingIntoPiecesOnly]:   { label: 'Wycinanie kawałków',  icon: 'pi pi-scissors' },
    [OperationType.CuttingIntoFramesOnly]:   { label: 'Podział na klatki',   icon: 'pi pi-th-large' },
    [OperationType.DownloadPiecesAndFrames]: { label: 'Pobieranie i cięcie', icon: 'pi pi-bolt' },
  };

  readonly statusConfig: Record<OperationStatus, { label: string; icon: string }> = {
    [OperationStatus.Undefined]:  { label: 'Nieznany',  icon: 'pi pi-question-circle' },
    [OperationStatus.Queued]:     { label: 'Oczekuje',  icon: 'pi pi-clock' },
    [OperationStatus.Processing]: { label: 'W toku',    icon: 'pi pi-spinner pi-spin' },
    [OperationStatus.Finished]:   { label: 'Ukończono', icon: 'pi pi-check-circle' },
    [OperationStatus.Error]:      { label: 'Błąd',      icon: 'pi pi-times-circle' },
  };

  constructor() {
    this.operationService.progressUpdates$
      .pipe(takeUntilDestroyed())
      .subscribe(update => this.applyProgress(update));

    this.operationService.statusUpdates$
      .pipe(takeUntilDestroyed())
      .subscribe(update => this.applyStatus(update));

    // Po zerwaniu połączenia mogliśmy przegapić część wiadomości - stan bierzemy ponownie z API.
    this.operationService.reconnected$
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.loadOperations());
  }

  // ── Lifecycle ─────────────────────────────────────────────────────────────────

  ngOnInit(): void {
    this.loadOperations();
    this.operationService.connect();
  }

  ngOnDestroy(): void {
    this.operationService.disconnect();
  }

  // ── Template helpers ──────────────────────────────────────────────────────────

  showProgress(operation: BaseOperationState): boolean {
    return (
      operation.operationStatus === OperationStatus.Processing ||
      (operation.operationStatus === OperationStatus.Queued && operation.progressPercentage > 0)
    );
  }

  // ── Data ──────────────────────────────────────────────────────────────────────

  private loadOperations(): void {
    if (this.loading()) return;

    this.loading.set(true);
    this.operationService.loadOperations().subscribe({
      next: data => {
        this.operations.set(data);
        this.loading.set(false);
      },
      error: err => {
        this.loading.set(false);
        this.toast.error(err);
      },
    });
  }

  private applyProgress(update: OperationProgressUpdate): void {
    if (!this.hasOperation(update.idOperation)) {
      this.loadOperations();
      return;
    }

    const progressPercentage = Math.min(100, Math.max(0, update.progressPercent));

    this.operations.update(operations =>
      operations.map(operation =>
        operation.idOperation === update.idOperation ? { ...operation, progressPercentage } : operation
      )
    );
  }

  private applyStatus(update: OperationStatusUpdate): void {
    if (!this.hasOperation(update.idOperation)) {
      this.loadOperations();
      return;
    }

    this.operations.update(operations =>
      operations.map(operation =>
        operation.idOperation === update.idOperation
          ? { ...operation, operationStatus: update.operationStatus }
          : operation
      )
    );
  }

  /** Wiadomość o nieznanej operacji oznacza, że pojawiła się nowa - wtedy dociągamy pełny stan z API. */
  private hasOperation(idOperation: number): boolean {
    return this.operations().some(operation => operation.idOperation === idOperation);
  }
}
