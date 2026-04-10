import { ProfileSourceType } from '@src/app/shared/enums/profile-source-type';

export interface Profile {
  idProfile: number;
  name: string;
  sources: ProfileSourceType[];
}