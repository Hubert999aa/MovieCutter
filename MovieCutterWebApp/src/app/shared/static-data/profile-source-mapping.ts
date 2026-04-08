import { ProfileSource } from '@shared/enums/profile-source';

export const profileSourceMap: Record<string, ProfileSource> = {
  'Youtube': ProfileSource.YouTube,
  'Instagram': ProfileSource.Instagram,
  'TikTok': ProfileSource.TikTok,
};
