import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'user-view',
    templateUrl: './user.component.html'
})

export class UserComponent implements OnInit {

    selectedPanel$: Observable<string>;

    constructor(private store: Store<any>) { 
        this.selectedPanel$ = this.store.select<string>(state => state.user.userState.selectedPanel);
    }

    ngOnInit() { }
}