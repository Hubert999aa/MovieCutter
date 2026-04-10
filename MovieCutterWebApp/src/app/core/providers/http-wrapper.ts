import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';

import { ErrorHandler } from '@core/providers/error-handler';
import { AppConfigurationLoader } from '@core/providers/app-configuration-loader';

@Injectable({ providedIn: 'root' })
export class HttpWrapper {
  private readonly httpClient = inject(HttpClient);
  private readonly errorHandler = inject(ErrorHandler);
  private readonly appConfigurationLoader = inject(AppConfigurationLoader);

  get<T>(url: string): Observable<T> {
    return this.httpClient.get<T>(this.appConfigurationLoader.apiBaseUrl + url).pipe(
      catchError(err => {
        this.errorHandler.logError(err);
        return throwError(() => err)
      })
    );
  }

  post<T>(url: string, body: Object): Observable<T> {
    return this.httpClient.post<T>(this.appConfigurationLoader.apiBaseUrl + url, body).pipe(
      catchError(err => {
        this.errorHandler.logError(err);
        return throwError(() => err)
      })
    );
  }

  put<T>(url: string, body: Object): Observable<T> {
    return this.httpClient.put<T>(this.appConfigurationLoader.apiBaseUrl + url, body).pipe(
      catchError(err => {
        this.errorHandler.logError(err);
        return throwError(() => err)
      })
    );
  }

  delete<T>(url: string, body: Object): Observable<T>{
    return this.httpClient.delete<T>(this.appConfigurationLoader.apiBaseUrl + url, { body: body }).pipe(
      catchError(err => {
        this.errorHandler.logError(err);
        return throwError(() => err)
      })
    );
  }
}