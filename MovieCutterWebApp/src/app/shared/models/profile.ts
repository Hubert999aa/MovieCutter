import { ProfileSource } from '@shared/enums/profile-source';

export interface Profile {
  idProfile: number;
  name: string;
  sources: ProfileSource[];
}