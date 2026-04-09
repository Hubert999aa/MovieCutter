import { ProfileSourceType } from '@src/app/shared/enums/profile-source-type';

export const profileSourceMap: Record<string, ProfileSourceType> = {
  'Youtube': ProfileSourceType.YouTube,
  'Instagram': ProfileSourceType.Instagram,
  'TikTok': ProfileSourceType.TikTok,
};
