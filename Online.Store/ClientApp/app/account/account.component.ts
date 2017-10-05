import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'account-view',
    template: '<router-outlet></router-outlet>'
})

export class AccountComponent implements OnInit {


    constructor() { }

    ngOnInit() { }
}