import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { ProfileApi } from './profile.api';
import { profileSourceMap } from '@shared/static-data/profile-source-mapping';
import { Profile } from '@shared/models/profile';
import { CreateProfileRequest } from '@shared/models/api-models/profile/create-profile.request';
import { CreateProfileResponse } from '@shared/models/api-models/profile/create-profile.response';
import { UpdateProfileRequest } from '@shared/models/api-models/profile/update-profile.request';


@Injectable({ providedIn: 'root' })
export class ProfileService {
  private readonly profileApi = inject(ProfileApi);

  loadProfiles(): Observable<Profile[]> {
    return this.profileApi.getProfiles().pipe(
      map(profiles =>
        profiles.map(profile => ({
          idProfile: profile.idProfile,
          name: profile.name,
          sources: profile.sources.map(source => profileSourceMap[source])
        }))
      )
    );
  }

  createProfile(request: CreateProfileRequest): Observable<CreateProfileResponse> {
    return this.profileApi.createProfile(request);
  }

  updateProfile(request: UpdateProfileRequest): Observable<Object> {
    return this.profileApi.updateProfile(request);
  }

  deleteProfile(idProfile: number): Observable<Object> {
    return this.profileApi.deleteProfile(idProfile);
  }
}
