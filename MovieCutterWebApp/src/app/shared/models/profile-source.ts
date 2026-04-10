import { ProfileSourceType } from '@src/app/shared/enums/profile-source-type';

export interface ProfileSource {
  id: number;
  type: ProfileSourceType;
  name: string;
  baseUrl: string;
}
