import { Component, ChangeDetectionStrategy, signal, computed, inject, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

import { Profile } from '@shared/models/profile';
import { ProfileSource } from '@shared/models/profile-source';
import { Video } from '@shared/models/video';
import { ProfileService } from '@shared/services/profiles/profile.services';
import { SourceService } from '@shared/services/sources/source.service';
import { ToastService } from '@shared/services/toast/toast.service';
import { VideoProcessingService } from '@shared/services/video-processing/video-processing.service';
import { LoaderComponent } from '@shared/components/loader/loader';
import { sourcesConfig } from '@shared/static-data/sources-config';
import { getAvatarColor, getInitials } from '@shared/helpers/avatar.helper';

export type DownloadMode = 'link' | 'profile';

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

  readonly canSubmitLink = computed(() => this.urlControl.valid);
  readonly canSubmitProfile = computed(() => this.selectedVideo() !== null);

  ngOnInit(): void {
    this.loadProfiles();
  }

  setMode(mode: DownloadMode): void {
    if (this.mode() === mode) return;
    this.mode.set(mode);
    this.resetProfileState();
    this.urlControl.reset();
  }

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
    this.loadVideos(source.id);
  }

  changeSource(): void {
    this.selectedSource.set(null);
    this.videos.set([]);
    this.selectedVideo.set(null);
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
    this.selectedVideo.set(
      this.selectedVideo()?.idVideo === video.idVideo ? null : video
    );
  }

  submitLink(): void {
    if (this.urlControl.invalid) return;
    this.videoProcessingService.requestVideoDownload(this.urlControl.value).subscribe({
      next: () => {
        this.router.navigate(['/pobieranie']);
      },
      error: err => {
        this.toast.error(err);
      },
    });
  }

  submitProfile(): void {
    const video = this.selectedVideo();
    const source = this.selectedSource();
    if (!video || !source) return;
    this.videoProcessingService.requestVideoDownload(video.url).subscribe({
      next: () => {
        this.router.navigate(['/pobieranie']);
      },
      error: err => {
        this.toast.error(err);
      },
    });
  }

  private resetProfileState(): void {
    this.selectedProfile.set(null);
    this.sources.set([]);
    this.selectedSource.set(null);
    this.videos.set([]);
    this.selectedVideo.set(null);
  }
}
