import { ProfileSource } from '@shared/enums/profile-source';

export const sourcesConfig: Record<ProfileSource, { icon: string; color: string; label: string }> = {
  [ProfileSource.YouTube]: { icon: 'pi pi-youtube', color: '#ef4444', label: 'YouTube' },
  [ProfileSource.Instagram]: { icon: 'pi pi-instagram', color: '#a855f7', label: 'Instagram' },
  [ProfileSource.TikTok]: { icon: '', color: '#e2e8f0', label: 'TikTok' },
};