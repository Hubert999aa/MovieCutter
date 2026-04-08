import { Injectable, inject } from '@angular/core';
import { Observable, of } from 'rxjs';

import { HttpWrapper } from '@core/providers/http-wrapper';
import { Profile } from '@shared/models/profile';
import { ProfileSource } from '@shared/enums/profile-source';
import { CreateProfileRequest } from '@shared/models/api-models/profile/create-profile.request';
import { CreateProfileResponse } from '@shared/models/api-models/profile/create-profile.response';
import { UpdateProfileRequest } from '@shared/models/api-models/profile/update-profile.request';

@Injectable({ providedIn: 'root' })
export class ProfileApi {
  private readonly httpWrapper = inject(HttpWrapper);
  private readonly profileBaseUrl = '/api/profile';

  getProfiles(): Observable<Profile[]> {
    return this.httpWrapper.get<Profile[]>(this.profileBaseUrl + '/getProfileList');
    return of([
      { idProfile: 1, name: 'Kanał Główny', sources: [ProfileSource.YouTube, ProfileSource.Instagram] },
      { idProfile: 2, name: 'Gaming Content', sources: [ProfileSource.YouTube, ProfileSource.TikTok] },
      { idProfile: 3, name: 'Social Media', sources: [ProfileSource.Instagram, ProfileSource.TikTok, ProfileSource.YouTube] },
    ]);
  }

  createProfile(request: CreateProfileRequest): Observable<CreateProfileResponse> {
    // TODO: return this.http.post<Profile>(this.baseUrl, request);
    return of({} as CreateProfileResponse);
  }

  updateProfile(request: UpdateProfileRequest): Observable<Object> {
    // TODO: return this.http.put<Profile>(`${this.baseUrl}/${request.idProfile}`, request);
    return of("");
  }

  deleteProfile(idProfile: number): Observable<Object> {
    // TODO: return this.http.delete<void>(`${this.baseUrl}/${idProfile}`);
    return of("");
  }
}
