import { Injectable, inject } from '@angular/core';
import { Observable, of } from 'rxjs';

import { HttpWrapper } from '@core/providers/http-wrapper';
import { Profile } from '@shared/models/profile';
import { CreateProfileRequest } from '@shared/models/api-models/profile/create-profile.request';
import { CreateProfileResponse } from '@shared/models/api-models/profile/create-profile.response';
import { UpdateProfileRequest } from '@shared/models/api-models/profile/update-profile.request';

@Injectable({ providedIn: 'root' })
export class ProfileApi {
  private readonly httpWrapper = inject(HttpWrapper);
  private readonly profileBaseUrl = '/api/profile';

  getProfiles(): Observable<Profile[]> {
    return this.httpWrapper.get<Profile[]>(this.profileBaseUrl + '/getProfileList');
  }

  createProfile(request: CreateProfileRequest): Observable<CreateProfileResponse> {
    return this.httpWrapper.post<CreateProfileResponse>(this.profileBaseUrl + '/createProfile', request);
  }

  updateProfile(request: UpdateProfileRequest): Observable<Object> {
    return this.httpWrapper.put<Object>(this.profileBaseUrl + '/updateProfile', request);
  }

  deleteProfile(idProfile: number): Observable<Object> {
    return this.httpWrapper.delete<Object>(this.profileBaseUrl + '/deleteProfile', { idProfile: idProfile });
  }
}
