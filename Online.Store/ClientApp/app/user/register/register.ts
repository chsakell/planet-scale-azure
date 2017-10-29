import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { RegisterVM } from '../../models/register-vm';
//import { Topic } from "../../models/topic";

@Component({
    selector: 'user-register-presentation',
    templateUrl: './register.html',
    styleUrls: ['./register.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class UserRegisterPresentationComponent {

    user: RegisterVM = { username : '', email: '', password: '', confirmPassword: '' };

    @Output() onRegister: EventEmitter<RegisterVM> = new EventEmitter();

    constructor() { }

    register() {
        this.onRegister.emit(this.user);
    }
}