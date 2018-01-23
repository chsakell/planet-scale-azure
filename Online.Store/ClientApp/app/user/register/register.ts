import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { RegisterVM } from '../../models/register-vm';
import { LoginVM } from '../../models/login-vm';

@Component({
    selector: 'user-register-presentation',
    templateUrl: './register.html',
    styleUrls: ['./register.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class UserRegisterPresentationComponent {

    user: RegisterVM = { username : '', email: '', password: '', confirmPassword: '', useIdentity: false };
    login: LoginVM = { username: '', password: '', rememberMe: true }

    @Input() useIdentity: boolean;
    @Output() onRegister: EventEmitter<RegisterVM> = new EventEmitter();
    @Output() onLogin: EventEmitter<LoginVM> = new EventEmitter();

    constructor() { }

    register() {
        this.user.useIdentity = this.useIdentity;
        this.onRegister.emit(this.user);
    }

    signIn() {
        this.onLogin.emit(this.login);
    }
}