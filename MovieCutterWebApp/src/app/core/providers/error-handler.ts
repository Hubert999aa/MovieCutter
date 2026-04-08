import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ErrorHandler {
  logError(err: any) {
    console.log('Błąd aplikacji:', err);
  }
}