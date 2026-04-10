import { Injectable } from '@angular/core';

declare global {
  interface Window {
    __appConfig?: {
      apiUrl?: string;
    };
  }
}

@Injectable({ providedIn: 'root' })
export class AppConfigurationLoader {
  get apiBaseUrl(): string {
    return window.__appConfig?.apiUrl ?? '';
  }
}