import { Component, ChangeDetectionStrategy, signal, inject, computed, OnInit, DestroyRef } from '@angular/core';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';

import { ProfileSourceType } from '@shared/enums/profile-source-type';
import { sourcesConfig } from '@shared/static-data/sources-config';
import { profileSourceTypeToSourceType } from '@shared/static-data/source-type-mapping';
import { ProfileSource } from '@shared/models/profile-source';
import { ProfileService } from '@shared/services/profiles/profile.services';
import { SourceService } from '@shared/services/sources/source.service';
import { getAvatarColor, getInitials } from '@shared/helpers/avatar.helper';

interface SourceTypeOption {
  label: string;
  value: ProfileSourceType;
  icon: string;
  color: string;
}

@Component({
  selector: 'app-profile-edit',
  imports: [RouterLink, ButtonModule, DialogModule, InputTextModule, SelectModule, ReactiveFormsModule],
  templateUrl: './profile-edit.html',
  styleUrl: './profile-edit.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileEditComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly profileService = inject(ProfileService);
  private readonly sourceService = inject(SourceService);

  private readonly idProfile = Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));
  private readonly profileName = (history.state as Record<string, unknown>)['profileName'] as string | undefined;
  private readonly isValid = !!this.idProfile && !!this.profileName;
  
  readonly sources = signal<ProfileSource[]>([]);
  readonly dialogVisible = signal(false);
  readonly editingSourceId = signal<number | null>(null);
  readonly pendingDeleteId = signal<number | null>(null);

  readonly platformConfig = sourcesConfig;
  readonly getAvatarColor = getAvatarColor;
  readonly getInitials = getInitials;
  readonly sourceTypeOptions: SourceTypeOption[] = Object.values(ProfileSourceType).map(type => ({
    label: sourcesConfig[type].label,
    value: type,
    icon: sourcesConfig[type].icon,
    color: sourcesConfig[type].color,
  }));

  readonly nameControl = new FormControl(this.profileName ?? '', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(64)],
  });

  readonly dialogHeader = computed(() =>
    this.editingSourceId() !== null ? 'Edytuj źródło' : 'Nowe źródło'
  );

  readonly sourceForm = new FormGroup({
    type: new FormControl<ProfileSourceType | null>(null, { validators: [Validators.required] }),
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(64)] }),
    baseUrl: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  ngOnInit(): void {
    if (!this.isValid) {
      this.router.navigate(['/profil']);
    };

    this.loadSources();
  }

  private loadSources(): void {
    this.sourceService.loadSources(this.idProfile).subscribe({
      next: sources => {
        this.sources.set(sources);
      },
      error: () => {
      // TODO: pokazać komunikat błędu
      },
    });
  }

  openAddDialog(): void {
    this.editingSourceId.set(null);
    this.sourceForm.reset();
    this.dialogVisible.set(true);
  }

  openEditDialog(source: ProfileSource): void {
    this.editingSourceId.set(source.id);
    this.sourceForm.setValue({ type: source.type, name: source.name, baseUrl: source.baseUrl });
    this.dialogVisible.set(true);
  }

  confirmSource(): void {
    if (this.sourceForm.invalid) return;
    const { type, name, baseUrl } = this.sourceForm.getRawValue();
    if (!type) return;

    const sourceType = profileSourceTypeToSourceType[type];
    const editId = this.editingSourceId();

    if (editId !== null) {
      this.sourceService.updateSource({ idSource: editId, idProfile: this.idProfile, name, baseUrl, sourceType })
        .subscribe({
          next: () => {
            this.loadSources();
            this.dialogVisible.set(false);
          },
          error: () => {
          // TODO: pokazać komunikat błędu
          },
        });
    } else {
      this.sourceService.createSource({ idProfile: this.idProfile, name, baseUrl, sourceType })
        .subscribe({
          next: created => {
            this.loadSources();
            this.dialogVisible.set(false);
          },
          error: () => {
          // TODO: pokazać komunikat błędu
          },
        });
    }
  }

  startDelete(id: number): void {
    this.pendingDeleteId.set(id);
  }

  cancelDelete(): void {
    this.pendingDeleteId.set(null);
  }

  confirmDelete(id: number): void {
    this.sourceService.deleteSource(id)
      .subscribe({
        next: () => {
          this.loadSources()
          this.pendingDeleteId.set(null);
        },
        error: () => this.pendingDeleteId.set(null),
      });
  }

  saveProfile(): void {
    if (this.nameControl.invalid) return;

    this.profileService.updateProfile({ idProfile: this.idProfile, name: this.nameControl.value }).subscribe({
      next: () => {
        this.loadSources();
      },
      error: () => {
      // TODO: pokazać komunikat błędu
      },
    });
  }
}
