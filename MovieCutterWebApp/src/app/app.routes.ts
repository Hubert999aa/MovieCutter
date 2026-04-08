import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/home/home').then(m => m.HomeComponent),
  },
  {
    path: 'pobieranie',
    loadComponent: () => import('./pages/download/download').then(m => m.DownloadComponent),
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
    path: '**',
    redirectTo: '',
  },
];
