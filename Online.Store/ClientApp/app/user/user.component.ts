import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { User } from '../models/user';

@Component({
    selector: 'user-view',
    templateUrl: './user.component.html'
})

export class UserComponent implements OnInit {

    selectedPanel$: Observable<string>;
    user$: Observable<User>;

    constructor(private store: Store<any>) { 
        this.selectedPanel$ = this.store.select<string>(state => state.user.userState.selectedPanel);
        this.user$ = this.store.select<User>(state => state.user.userState.user);
    }

    ngOnInit() { }
}