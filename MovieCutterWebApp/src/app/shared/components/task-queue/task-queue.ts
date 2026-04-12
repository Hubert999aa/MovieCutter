import { Component, ChangeDetectionStrategy, signal, computed } from '@angular/core';

export type TaskStatus = 'queued' | 'processing' | 'done' | 'error';
export type TaskType = 'download' | 'frames' | 'pieces';

export interface TaskItem {
  id: string;
  type: TaskType;
  name: string;
  status: TaskStatus;
  progress?: number; // 0–100, only relevant for 'processing'
}

@Component({
  selector: 'app-task-queue',
  templateUrl: './task-queue.html',
  styleUrl: './task-queue.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskQueueComponent {
  // TODO: inject TaskService and replace with real data when API is ready
  readonly tasks = signal<TaskItem[]>([
    { id: '1', type: 'download', name: 'interview_final.mp4',  status: 'processing', progress: 65 },
    { id: '2', type: 'pieces',   name: 'webinar_2024.mp4',     status: 'queued' },
    { id: '3', type: 'frames',   name: 'clip_showcase.mp4',    status: 'done' },
    { id: '4', type: 'download', name: 'tutorial_part2.mp4',   status: 'error' },
  ]);

  readonly activeCount = computed(() =>
    this.tasks().filter(t => t.status === 'queued' || t.status === 'processing').length
  );

  readonly typeConfig: Record<TaskType, { label: string; icon: string }> = {
    download: { label: 'Pobieranie',         icon: 'pi pi-download' },
    frames:   { label: 'Podział na klatki',  icon: 'pi pi-th-large' },
    pieces:   { label: 'Wycinanie kawałków', icon: 'pi pi-scissors' },
  };

  readonly statusConfig: Record<TaskStatus, { label: string; icon: string }> = {
    queued:     { label: 'Oczekuje',  icon: 'pi pi-clock' },
    processing: { label: 'W toku',    icon: 'pi pi-spinner pi-spin' },
    done:       { label: 'Ukończono', icon: 'pi pi-check-circle' },
    error:      { label: 'Błąd',      icon: 'pi pi-times-circle' },
  };
}
