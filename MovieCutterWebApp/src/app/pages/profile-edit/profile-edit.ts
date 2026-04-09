import { Component, ChangeDetectionStrategy, signal, inject, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';

import { ProfileSourceType } from '@src/app/shared/enums/profile-source-type';
import { sourcesConfig } from '@shared/static-data/sources-config';
import { avatarColors } from '@shared/static-data/avatar-colors';
import { ProfileSource } from '@src/app/shared/models/profile-source';

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
export class ProfileEditComponent {
  readonly platformConfig = sourcesConfig;

  readonly sourceTypeOptions: SourceTypeOption[] = Object.values(ProfileSourceType).map(type => ({
    label: sourcesConfig[type].label,
    value: type,
    icon: sourcesConfig[type].icon,
    color: sourcesConfig[type].color,
  }));

  readonly nameControl = new FormControl('Kanał Główny', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(64)],
  });

  readonly sources = signal<ProfileSource[]>([
    { id: 1, type: ProfileSourceType.YouTube, name: 'Kanał główny', baseUrl: 'https://www.youtube.com/@mychannel' },
    { id: 2, type: ProfileSourceType.Instagram, name: 'Profil Instagram', baseUrl: 'https://www.instagram.com/mychannel' },
  ]);

  readonly dialogVisible = signal(false);
  readonly editingSourceId = signal<number | null>(null);
  readonly pendingDeleteId = signal<number | null>(null);

  readonly dialogHeader = computed(() =>
    this.editingSourceId() !== null ? 'Edytuj źródło' : 'Nowe źródło'
  );

  readonly sourceForm = new FormGroup({
    type: new FormControl<ProfileSourceType | null>(null, { validators: [Validators.required] }),
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(64)] }),
    baseUrl: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  private nextId = 3;

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

    const editId = this.editingSourceId();
    if (editId !== null) {
      this.sources.update(list =>
        list.map(s => s.id === editId ? { ...s, type, name, baseUrl } : s)
      );
    } else {
      this.sources.update(list => [...list, { id: this.nextId++, type, name, baseUrl }]);
    }
    this.dialogVisible.set(false);
  }

  startDelete(id: number): void {
    this.pendingDeleteId.set(id);
  }

  cancelDelete(): void {
    this.pendingDeleteId.set(null);
  }

  confirmDelete(id: number): void {
    this.sources.update(list => list.filter(s => s.id !== id));
    this.pendingDeleteId.set(null);
  }

  saveProfile(): void {
    if (this.nameControl.invalid) return;
    // TODO: podłączyć do API
  }

  getAvatarColor(name: string): string {
    let hash = 0;
    for (let i = 0; i < name.length; i++) hash += name.charCodeAt(i);
    return avatarColors[hash % avatarColors.length];
  }

  getInitials(name: string): string {
    return name.split(' ').map(w => w[0]).filter(Boolean).join('').toUpperCase().slice(0, 2) || '?';
  }
}
