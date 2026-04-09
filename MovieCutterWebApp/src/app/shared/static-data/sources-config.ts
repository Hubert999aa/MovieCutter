import { ProfileSourceType } from '@src/app/shared/enums/profile-source-type';

export const sourcesConfig: Record<ProfileSourceType, { icon: string; color: string; label: string }> = {
  [ProfileSourceType.YouTube]: { icon: 'pi pi-youtube', color: '#ef4444', label: 'YouTube' },
  [ProfileSourceType.Instagram]: { icon: 'pi pi-instagram', color: '#a855f7', label: 'Instagram' },
  [ProfileSourceType.TikTok]: { icon: '', color: '#e2e8f0', label: 'TikTok' },
};