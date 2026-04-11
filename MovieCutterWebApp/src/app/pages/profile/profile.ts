import { Component, ChangeDetectionStrategy, signal, inject, OnInit, WritableSignal } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';

import { ProfileService } from '@src/app/shared/services/profile/profile.services';
import { sourcesConfig } from '@shared/static-data/sources-config';
import { Profile } from '@shared/models/profile';
import { getAvatarColor, getInitials } from '@shared/helpers/avatar.helper';
import { LoaderComponent } from '@shared/components/loader/loader';
import { ToastService } from '@shared/services/toast/toast.service';

@Component({
  selector: 'app-profile',
  imports: [ButtonModule, DialogModule, InputTextModule, ReactiveFormsModule, LoaderComponent],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileComponent implements OnInit {
  private readonly router  = inject(Router);
  private readonly profileService = inject(ProfileService);
  private readonly toast = inject(ToastService);
  readonly platformConfig = sourcesConfig;
  readonly getAvatarColor = getAvatarColor;
  readonly getInitials = getInitials;

  dialogVisible = signal(false);
  profiles: WritableSignal<Profile[]> = signal([]);
  pendingDeleteId: WritableSignal<number | null> = signal(null);
  readonly profilesLoading = signal(false);
  nameControl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required],
  });

  ngOnInit(): void {
    this.loadProfiles();
  }

  loadProfiles(): void {
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

  openAddDialog(): void {
    this.nameControl.reset();
    this.dialogVisible.set(true);
  }

  confirmAdd(): void {
    if (this.nameControl.invalid) return;

    const profileName = this.nameControl.value.trim();
    this.profileService.createProfile({ name: profileName }).subscribe({
      next: createdProfileId => {
        this.dialogVisible.set(false);
        this.router.navigate(['/profil', createdProfileId], {
          state: { profileName },
        });
      },
      error: err => {
        this.toast.error(err);
      },
    });
  }

  editProfile(profile: Profile): void {
    this.router.navigate(['/profil', profile.idProfile], {
      state: { profileName: profile.name },
    });
  }

  startDelete(idProfile: number): void {
    this.pendingDeleteId.set(idProfile);
  }

  cancelDelete(): void {
    this.pendingDeleteId.set(null);
  }

  confirmDelete(idProfile: number): void {
    this.profileService.deleteProfile(idProfile).subscribe({
      next: () => {
        this.pendingDeleteId.set(null);
        this.loadProfiles();
      },
      error: err => {
        this.toast.error(err);
        this.pendingDeleteId.set(null);
      },
    });
  }
}
