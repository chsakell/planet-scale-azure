import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
//import { Topic } from "../../models/topic";

@Component({
    selector: 'account-login-presentation',
    templateUrl: './login.html',
    styleUrls: ['./login.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class AccountLoginPresentationComponent {

    //@Input() topics: Topic[];

    constructor() { }

}