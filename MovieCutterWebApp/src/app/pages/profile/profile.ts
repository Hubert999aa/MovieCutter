import { Component, ChangeDetectionStrategy, signal, inject, OnInit, WritableSignal } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';

import { ProfileService } from '@shared/services/profiles/profile.services';
import { sourcesConfig } from '@shared/static-data/sources-config';
import { Profile } from '@shared/models/profile';
import { getAvatarColor, getInitials } from '@shared/helpers/avatar.helper';

@Component({
  selector: 'app-profile',
  imports: [ButtonModule, DialogModule, InputTextModule, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileComponent implements OnInit {
  private readonly router  = inject(Router);
  private readonly profileService = inject(ProfileService);
  readonly platformConfig = sourcesConfig;
  readonly getAvatarColor = getAvatarColor;
  readonly getInitials = getInitials;
  
  dialogVisible = signal(false);
  profiles: WritableSignal<Profile[]> = signal([]);
  pendingDeleteId: WritableSignal<number | null> = signal(null);
  nameControl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required],
  });

  ngOnInit(): void {
    this.loadProfiles();
  }

  loadProfiles(): void {
    this.profileService.loadProfiles().subscribe(data => {
      this.profiles.set(data)
    })
  }

  openAddDialog(): void {
    this.nameControl.reset();
    this.dialogVisible.set(true);
  }

  confirmAdd(): void {
    if (this.nameControl.invalid) return;

    this.profileService.createProfile({ name: this.nameControl.value.trim() }).subscribe({
      next: createdProfileId => {
        this.dialogVisible.set(false);
        this.router.navigate(['/profil', createdProfileId]);
      },
      error: () => {
        // TODO: pokazać komunikat błędu
      },
    });
  }

  editProfile(idProfile: number): void {
    this.router.navigate(['/profil', idProfile]);
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
      error: () => {
        // TODO: pokazać komunikat błędu
        this.pendingDeleteId.set(null);
      },
    });
  }
}
