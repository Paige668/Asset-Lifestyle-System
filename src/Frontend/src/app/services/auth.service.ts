import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private userSubject = new BehaviorSubject<string | null>(localStorage.getItem('user'));
    user$ = this.userSubject.asObservable();

    constructor(private router: Router) { }

    login(username: string, role: 'Admin' | 'User') {
        // Mock Login
        localStorage.setItem('user', username);
        localStorage.setItem('role', role);
        this.userSubject.next(username);
        this.router.navigate(['/assets']);
    }

    logout() {
        localStorage.removeItem('user');
        localStorage.removeItem('role');
        this.userSubject.next(null);
        this.router.navigate(['/login']);
    }

    getRole(): string | null {
        return localStorage.getItem('role');
    }

    isLoggedIn(): boolean {
        return !!localStorage.getItem('user');
    }
}
