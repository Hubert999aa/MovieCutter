import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-download',
  imports: [RouterLink, ButtonModule],
  templateUrl: './download.html',
  styleUrl: './download.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DownloadComponent {}
