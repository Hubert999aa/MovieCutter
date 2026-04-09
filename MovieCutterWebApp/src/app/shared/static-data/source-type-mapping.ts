import { ProfileSourceType } from '@shared/enums/profile-source-type';

// Matches backend: NotDefined = 0, Youtube = 1, Instagram = 2, TikTok = 3
export const sourceTypeToProfileSourceType: Record<number, ProfileSourceType> = {
  1: ProfileSourceType.YouTube,
  2: ProfileSourceType.Instagram,
  3: ProfileSourceType.TikTok,
};

export const profileSourceTypeToSourceType: Record<ProfileSourceType, number> = {
  [ProfileSourceType.YouTube]: 1,
  [ProfileSourceType.Instagram]: 2,
  [ProfileSourceType.TikTok]: 3,
};

export const profileSourceMap: Record<string, ProfileSourceType> = {
  'Youtube': ProfileSourceType.YouTube,
  'Instagram': ProfileSourceType.Instagram,
  'TikTok': ProfileSourceType.TikTok,
};