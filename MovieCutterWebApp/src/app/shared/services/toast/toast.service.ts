import { Injectable, inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageService } from 'primeng/api';

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly messageService = inject(MessageService);

  error(err: unknown): void {
    let summary = 'Błąd';
    let detail = 'Wystąpił nieoczekiwany błąd.';

    if (err instanceof HttpErrorResponse) {
      summary = `Błąd ${err.status}`;

      const body = err.error;
      if (body && typeof body === 'object' && typeof body['message'] === 'string' && body['message']) {
        detail = body['message'];
      } else if (typeof body === 'string' && body.trim().length > 0) {
        detail = body.trim();
      } else if (err.message) {
        detail = err.message;
      }
    }

    this.messageService.add({ severity: 'error', summary, detail, life: 6000 });
  }

  success(summary: string, detail?: string): void {
    this.messageService.add({ severity: 'success', summary, detail, life: 4000 });
  }
}
