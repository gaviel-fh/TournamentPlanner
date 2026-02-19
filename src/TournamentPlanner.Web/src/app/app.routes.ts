import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { guestGuard } from './core/auth/guest.guard';

export const routes: Routes = [
  {
    path: 'auth/login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login-page.component').then((m) => m.LoginPageComponent),
  },
  {
    path: 'auth/register',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/register-page.component').then((m) => m.RegisterPageComponent),
  },
  {
    path: 'app',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/tournaments/tournaments-page.component').then(
        (m) => m.TournamentsPageComponent,
      ),
  },
  { path: '', pathMatch: 'full', redirectTo: 'app' },
  { path: '**', redirectTo: 'auth/login' },
];
