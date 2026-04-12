import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/home/home').then(m => m.HomeComponent),
  },
  {
    path: 'pobieranie',
    loadComponent: () => import('./pages/request-download/request-download').then(m => m.RequestDownloadComponent),
  },
  {
    path: 'edytor',
    loadComponent: () => import('./pages/cutting/cutting').then(m => m.CuttingComponent),
  },
  {
    path: 'profil',
    loadComponent: () => import('./pages/profile/profile').then(m => m.ProfileComponent),
  },
  {
    path: 'profil/:id',
    loadComponent: () => import('./pages/profile-edit/profile-edit').then(m => m.ProfileEditComponent),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
