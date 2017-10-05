import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
//import { Topic } from "../../models/topic";

@Component({
    selector: 'account-register-presentation',
    templateUrl: './register.html',
    styleUrls: ['./register.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class AccountRegisterPresentationComponent {

    //@Input() topics: Topic[];

    constructor() { }

}