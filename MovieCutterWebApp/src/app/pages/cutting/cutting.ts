import { Component, ChangeDetectionStrategy, signal, computed, inject, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { startWith, map } from 'rxjs';
import { ButtonModule } from 'primeng/button';

import { FilesService } from '@shared/services/files/files.service';
import { VideoProcessingService } from '@shared/services/video-processing/video-processing.service';
import { ToastService } from '@shared/services/toast/toast.service';
import { LoaderComponent } from '@shared/components/loader/loader';
import { FileMetadata } from '@shared/models/file-metadata';

export type CuttingMode = 'frames' | 'pieces';

const TIME_PATTERN = /^\d{2}:\d{2}:\d{2}$/;

function timeFormatValidator(): ValidatorFn {
  return (control: AbstractControl) => {
    const v = control.value as string;
    if (!v) return null;
    return TIME_PATTERN.test(v) ? null : { timeFormat: true };
  };
}

type PieceGroup = FormGroup<{
  startTime: FormControl<string>;
  endTime: FormControl<string>;
}>;

@Component({
  selector: 'app-cutting',
  imports: [ButtonModule, ReactiveFormsModule, LoaderComponent],
  templateUrl: './cutting.html',
  styleUrl: './cutting.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CuttingComponent implements OnInit {
  private readonly filesService = inject(FilesService);
  private readonly videoProcessingService = inject(VideoProcessingService);
  private readonly toast = inject(ToastService);

  // ── Files ─────────────────────────────────────────────────────────────────────
  readonly files = signal<FileMetadata[]>([]);
  readonly filesLoading = signal(false);
  readonly selectedFile = signal<FileMetadata | null>(null);

  // ── Mode ─────────────────────────────────────────────────────────────────────
  readonly mode = signal<CuttingMode>('frames');

  // ── Submitting ────────────────────────────────────────────────────────────────
  readonly submitting = signal(false);

  // ── Pieces form ───────────────────────────────────────────────────────────────
  readonly form = new FormGroup({
    pieces: new FormArray<PieceGroup>([]),
  });

  get piecesArray(): FormArray<PieceGroup> {
    return this.form.get('pieces') as FormArray<PieceGroup>;
  }

  private readonly formStatus = toSignal(
    this.form.statusChanges.pipe(
      startWith(this.form.status),
      map(() => this.form.status),
    ),
    { initialValue: this.form.status },
  );

  readonly canSubmit = computed(() => {
    if (!this.selectedFile() || this.submitting()) return false;
    if (this.mode() === 'frames') return true;
    return this.formStatus() === 'VALID' && this.piecesArray.length > 0;
  });

  // ── Lifecycle ─────────────────────────────────────────────────────────────────
  ngOnInit(): void {
    this.loadFiles();
    this.addPiece();
  }

  // ── Files ─────────────────────────────────────────────────────────────────────
  private loadFiles(): void {
    this.filesLoading.set(true);
    this.filesService.getFilesMetadata().subscribe({
      next: data => {
        this.files.set(data);
        this.filesLoading.set(false);
      },
      error: err => {
        this.filesLoading.set(false);
        this.toast.error(err);
      },
    });
  }

  selectFile(file: FileMetadata): void {
    this.selectedFile.set(file);
  }

  changeFile(): void {
    this.selectedFile.set(null);
  }

  // ── Mode ─────────────────────────────────────────────────────────────────────
  setMode(mode: CuttingMode): void {
    if (this.mode() === mode) return;
    this.mode.set(mode);
  }

  // ── Pieces ────────────────────────────────────────────────────────────────────
  private createPieceGroup(): PieceGroup {
    return new FormGroup({
      startTime: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, timeFormatValidator()],
      }),
      endTime: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, timeFormatValidator()],
      }),
    });
  }

  addPiece(): void {
    this.piecesArray.push(this.createPieceGroup());
  }

  removePiece(index: number): void {
    this.piecesArray.removeAt(index);
  }

  // ── Submit ────────────────────────────────────────────────────────────────────
  submit(): void {
    const file = this.selectedFile();
    if (!file || !this.canSubmit() || this.submitting()) return;

    this.submitting.set(true);

    if (this.mode() === 'frames') {
      this.videoProcessingService.requestVideoCuttingIntoFrames(file.fullPath).subscribe({
        next: () => {
          this.toast.success('Zlecono przetwarzanie', 'Plik zostanie podzielony na klatki.');
          this.reset();
        },
        error: err => {
          this.submitting.set(false);
          this.toast.error(err);
        },
      });
    } else {
      const videoPices = this.piecesArray.getRawValue().map(p => ({
        startTime: p.startTime,
        endTime: p.endTime,
      }));

      this.videoProcessingService.requestVideoCuttingIntoPices({
        sourceVideoFullPath: file.fullPath,
        videoPices,
      }).subscribe({
        next: () => {
          this.toast.success('Zlecono przetwarzanie', `Zlecono ${videoPices.length} kawałek(ów) do wycięcia.`);
          this.reset();
        },
        error: err => {
          this.submitting.set(false);
          this.toast.error(err);
        },
      });
    }
  }

  private reset(): void {
    this.submitting.set(false);
    this.selectedFile.set(null);
    this.mode.set('frames');
    this.piecesArray.clear();
    this.addPiece();
  }
}
