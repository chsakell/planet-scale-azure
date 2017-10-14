import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { LoginVM } from '../../models/login-vm';

@Component({
    selector: 'user-login-presentation',
    templateUrl: './login.html',
    styleUrls: ['./login.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class UserLoginPresentationComponent {

    user: LoginVM = { username: '', password: '', rememberMe: false };

    @Output() onLogin: EventEmitter<LoginVM> = new EventEmitter();

    constructor() { }

    login() {
        this.onLogin.emit(this.user);
    }

}