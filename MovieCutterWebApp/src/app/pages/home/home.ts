import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TaskQueueComponent } from '@shared/components/task-queue/task-queue';

@Component({
  selector: 'app-home',
  imports: [RouterLink, ButtonModule, TaskQueueComponent],
  templateUrl: './home.html',
  styleUrl: './home.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent {
  readonly filmHoles = Array(20);
}
