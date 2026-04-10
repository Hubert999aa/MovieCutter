import { Component, ChangeDetectionStrategy, input } from '@angular/core';

export type LoaderVariant = 'cards' | 'rows';

@Component({
  selector: 'app-loader',
  template: `
    @if (variant() === 'cards') {
      <ul class="loader-cards" aria-label="Ładowanie danych…" aria-busy="true">
        @for (_ of items(); track $index) {
          <li class="loader-card" aria-hidden="true">
            <div class="lc-main">
              <div class="lc-avatar shimmer"></div>
              <div class="lc-info">
                <div class="lc-name shimmer"></div>
                <div class="lc-badges">
                  <div class="lc-badge shimmer"></div>
                  <div class="lc-badge shimmer"></div>
                </div>
              </div>
            </div>
            <div class="lc-actions">
              <div class="lc-btn shimmer"></div>
              <div class="lc-btn-sm shimmer"></div>
            </div>
          </li>
        }
      </ul>
    }

    @if (variant() === 'rows') {
      <ul class="loader-rows" aria-label="Ładowanie danych…" aria-busy="true">
        @for (_ of items(); track $index) {
          <li class="loader-row" aria-hidden="true">
            <div class="lr-icon shimmer"></div>
            <div class="lr-info">
              <div class="lr-line lr-line--wide shimmer"></div>
              <div class="lr-line lr-line--narrow shimmer"></div>
            </div>
            <div class="lr-actions">
              <div class="lr-btn shimmer"></div>
              <div class="lr-btn-sm shimmer"></div>
            </div>
          </li>
        }
      </ul>
    }
  `,
  styleUrl: './loader.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoaderComponent {
  readonly variant = input<LoaderVariant>('rows');
  readonly count = input<number>(3);

  protected items(): number[] {
    return Array.from({ length: this.count() });
  }
}
