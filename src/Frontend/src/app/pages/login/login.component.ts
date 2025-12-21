import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './login.component.html'
})
export class LoginComponent {
    username = '';

    constructor(private auth: AuthService) { }

    login(role: 'Admin' | 'User') {
        if (!this.username) return;
        this.auth.login(this.username, role);
    }
}
