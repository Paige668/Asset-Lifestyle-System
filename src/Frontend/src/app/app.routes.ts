import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { AssetListComponent } from './pages/asset-list/asset-list.component';
import { inject } from '@angular/core';
import { AuthService } from './services/auth.service';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'assets', component: AssetListComponent, canActivate: [() => inject(AuthService).isLoggedIn()] },
    { path: 'audit', loadComponent: () => import('./pages/audit-log/audit-log.component').then(m => m.AuditLogComponent), canActivate: [() => inject(AuthService).isLoggedIn()] },
    { path: '', redirectTo: 'login', pathMatch: 'full' }
];
