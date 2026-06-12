import { Component, ChangeDetectionStrategy, signal, computed, inject, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  AbstractControl,
  FormArray,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { map, startWith } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

import { Profile } from '@shared/models/profile';
import { ProfileSource } from '@shared/models/profile-source';
import { Video } from '@shared/models/video';
import { ProfileService } from '@src/app/shared/services/profile/profile.service';
import { SourceService } from '@src/app/shared/services/source/source.service';
import { ToastService } from '@shared/services/toast/toast.service';
import { VideoProcessingService } from '@shared/services/video-processing/video-processing.service';
import { LoaderComponent } from '@shared/components/loader/loader';
import { sourcesConfig } from '@shared/static-data/sources-config';
import { getAvatarColor, getInitials } from '@shared/helpers/avatar.helper';

export type DownloadMode = 'link' | 'profile';
export type ProcessingAction = 'download-only' | 'download-and-cut';
export type CuttingMode = 'frames' | 'pieces';

type PieceGroup = FormGroup<{
  startTime: FormControl<string>;
  endTime: FormControl<string>;
}>;

function timeFormatValidator(): ValidatorFn {
  return (control: AbstractControl) => {
    const v = control.value as string;
    if (!v) return null;
    return /^\d{2}:\d{2}:\d{2}$/.test(v) ? null : { timeFormat: true };
  };
}

@Component({
  selector: 'app-request-download',
  imports: [RouterLink, ButtonModule, InputTextModule, ReactiveFormsModule, LoaderComponent],
  templateUrl: './request-download.html',
  styleUrl: './request-download.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RequestDownloadComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly profileService = inject(ProfileService);
  private readonly sourceService = inject(SourceService);
  private readonly videoProcessingService = inject(VideoProcessingService);
  private readonly toast = inject(ToastService);

  readonly platformConfig = sourcesConfig;
  readonly getAvatarColor = getAvatarColor;
  readonly getInitials = getInitials;

  // ── Download mode ────────────────────────────────────────────────────────────

  readonly mode = signal<DownloadMode>('link');

  readonly urlControl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required],
  });

  readonly profiles = signal<Profile[]>([]);
  readonly profilesLoading = signal(false);
  readonly selectedProfile = signal<Profile | null>(null);

  readonly sources = signal<ProfileSource[]>([]);
  readonly sourcesLoading = signal(false);
  readonly selectedSource = signal<ProfileSource | null>(null);

  readonly videos = signal<Video[]>([]);
  readonly videosLoading = signal(false);
  readonly selectedVideo = signal<Video | null>(null);

  // ── Processing options ───────────────────────────────────────────────────────

  readonly processingAction = signal<ProcessingAction>('download-only');
  readonly cuttingMode = signal<CuttingMode>('frames');
  readonly submitting = signal(false);

  readonly form = new FormGroup({
    pieces: new FormArray<PieceGroup>([]),
  });

  get piecesArray(): FormArray<PieceGroup> {
    return this.form.get('pieces') as FormArray<PieceGroup>;
  }

  private readonly urlStatus = toSignal(
    this.urlControl.statusChanges.pipe(startWith(this.urlControl.status)),
    { initialValue: this.urlControl.status }
  );

  private readonly piecesFormStatus = toSignal(
    this.form.statusChanges.pipe(startWith(this.form.status), map(() => this.form.status)),
    { initialValue: this.form.status }
  );

  readonly showProcessingSection = computed(() =>
    this.mode() === 'link' ? this.urlStatus() === 'VALID' : this.selectedVideo() !== null
  );

  readonly canSubmit = computed(() => {
    if (this.submitting() || !this.showProcessingSection()) return false;
    if (this.processingAction() === 'download-only') return true;
    if (this.cuttingMode() === 'frames') return true;
    return this.piecesFormStatus() === 'VALID' && this.piecesArray.length > 0;
  });

  // ── Lifecycle ────────────────────────────────────────────────────────────────

  ngOnInit(): void {
    this.loadProfiles();
    this.addPiece();
  }

  // ── Mode control ─────────────────────────────────────────────────────────────

  setMode(mode: DownloadMode): void {
    if (this.mode() === mode) return;
    this.mode.set(mode);
    this.resetProfileState();
    this.urlControl.reset();
    this.resetProcessing();
  }

  // ── Processing control ───────────────────────────────────────────────────────

  setProcessingAction(action: ProcessingAction): void {
    this.processingAction.set(action);
  }

  setCuttingMode(mode: CuttingMode): void {
    this.cuttingMode.set(mode);
  }

  addPiece(): void {
    this.piecesArray.push(
      new FormGroup({
        startTime: new FormControl('', {
          nonNullable: true,
          validators: [Validators.required, timeFormatValidator()],
        }),
        endTime: new FormControl('', {
          nonNullable: true,
          validators: [Validators.required, timeFormatValidator()],
        }),
      }) as PieceGroup
    );
  }

  removePiece(index: number): void {
    this.piecesArray.removeAt(index);
  }

  // ── Submit ───────────────────────────────────────────────────────────────────

  submit(): void {
    if (!this.canSubmit()) return;
    if (this.processingAction() === 'download-only') {
      this.submitDownloadOnly();
    } else {
      this.submitDownloadAndCut();
    }
  }

  private getUrl(): string {
    return this.mode() === 'link'
      ? this.urlControl.value
      : (this.selectedVideo()?.url ?? '');
  }

  private submitDownloadOnly(): void {
    const url = this.getUrl();
    if (!url) return;
    this.submitting.set(true);
    this.videoProcessingService.requestVideoDownload(url).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigate(['/']);
      },
      error: err => {
        this.submitting.set(false);
        this.toast.error(err);
      },
    });
  }

  private submitDownloadAndCut(): void {
    const url = this.getUrl();
    if (!url) return;

    const cutVideoInOnePiece = this.cuttingMode() === 'frames';
    const videoPices =
      this.cuttingMode() === 'frames'
        ? []
        : this.piecesArray.controls.map(g => ({
            startTime: g.controls.startTime.value,
            endTime: g.controls.endTime.value,
          }));

    this.submitting.set(true);
    this.videoProcessingService
      .requestDownloadAndCutVideo({ url, cutVideoInOnePiece, videoPices })
      .subscribe({
        next: () => {
          this.submitting.set(false);
          this.router.navigate(['/']);
        },
        error: err => {
          this.submitting.set(false);
          this.toast.error(err);
        },
      });
  }

  private resetProcessing(): void {
    this.processingAction.set('download-only');
    this.cuttingMode.set('frames');
    while (this.piecesArray.length > 0) {
      this.piecesArray.removeAt(0);
    }
    this.addPiece();
  }

  // ── Profile flow ─────────────────────────────────────────────────────────────

  private loadProfiles(): void {
    this.profilesLoading.set(true);
    this.profileService.loadProfiles().subscribe({
      next: data => {
        this.profiles.set(data);
        this.profilesLoading.set(false);
      },
      error: err => {
        this.profilesLoading.set(false);
        this.toast.error(err);
      },
    });
  }

  selectProfile(profile: Profile): void {
    this.selectedProfile.set(profile);
    this.sources.set([]);
    this.selectedSource.set(null);
    this.videos.set([]);
    this.selectedVideo.set(null);
    this.resetProcessing();
    this.loadSources(profile.idProfile);
  }

  changeProfile(): void {
    this.resetProfileState();
  }

  private loadSources(idProfile: number): void {
    this.sourcesLoading.set(true);
    this.sourceService.loadSources(idProfile).subscribe({
      next: data => {
        this.sources.set(data);
        this.sourcesLoading.set(false);
      },
      error: err => {
        this.sourcesLoading.set(false);
        this.toast.error(err);
      },
    });
  }

  selectSource(source: ProfileSource): void {
    this.selectedSource.set(source);
    this.videos.set([]);
    this.selectedVideo.set(null);
    this.resetProcessing();
    this.loadVideos(source.id);
  }

  changeSource(): void {
    this.selectedSource.set(null);
    this.videos.set([]);
    this.selectedVideo.set(null);
    this.resetProcessing();
  }

  private loadVideos(idSource: number): void {
    this.videosLoading.set(true);
    this.sourceService.loadLastVideos(idSource).subscribe({
      next: data => {
        this.videos.set(data);
        this.videosLoading.set(false);
      },
      error: err => {
        this.videosLoading.set(false);
        this.toast.error(err);
      },
    });
  }

  selectVideo(video: Video): void {
    const wasSelected = this.selectedVideo()?.idVideo === video.idVideo;
    this.selectedVideo.set(wasSelected ? null : video);
    if (wasSelected) this.resetProcessing();
  }

  private resetProfileState(): void {
    this.selectedProfile.set(null);
    this.sources.set([]);
    this.selectedSource.set(null);
    this.videos.set([]);
    this.selectedVideo.set(null);
    this.resetProcessing();
  }
}
