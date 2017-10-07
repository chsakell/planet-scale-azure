import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { RegisterVM } from '../../models/register-vm';
//import { Topic } from "../../models/topic";

@Component({
    selector: 'account-register-presentation',
    templateUrl: './register.html',
    styleUrls: ['./register.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class AccountRegisterPresentationComponent {

    _user: RegisterVM;
    @Input() 
    set user(val : RegisterVM)
    {
        this._user = Object.assign({}, val);
    }

    get user() {
        return this._user;
    }

    @Output() onRegister: EventEmitter<RegisterVM> = new EventEmitter();

    constructor() { }

    register() {
        this.onRegister.emit(this.user);
    }
}