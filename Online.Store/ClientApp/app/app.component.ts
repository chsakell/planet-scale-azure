import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { UserState } from './user/store/user.state';
import * as userActions from './user/store/user.actions';
import { Observable } from 'rxjs/Observable';
import { Cart } from './models/cart';
import { User } from './models/user';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

    cart$: Observable<Cart>;
    user$: Observable<User>;

    constructor(private store: Store<any>) {
        this.cart$ = this.store.select<Cart>(state => state.user.userState.cart);
        this.user$ = this.store.select<User>(state => state.user.userState.user);
     }

    ngOnInit(): void {
        this.store.dispatch(new userActions.GetCartAction());
    }

}
