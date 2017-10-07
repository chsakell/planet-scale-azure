import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { LoginVM } from '../../models/login-vm';

@Component({
    selector: 'account-login-presentation',
    templateUrl: './login.html',
    styleUrls: ['./login.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class AccountLoginPresentationComponent {

    _user: LoginVM;
    @Input() 
    set user(val : LoginVM)
    {
        this._user = Object.assign({}, val);
    }

    get user() {
        return this._user;
    }

    @Output() onLogin: EventEmitter<LoginVM> = new EventEmitter();

    constructor() { }

    login() {
        this.onLogin.emit(this.user);
    }

}