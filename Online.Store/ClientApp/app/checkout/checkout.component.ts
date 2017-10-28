import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Cart } from "../models/cart";
import * as userActions from '../user/store/user.actions';

@Component({
    selector: 'checkout-order',
    templateUrl: './checkout.component.html',

})

export class CheckoutComponent implements OnInit {

    cart$: Observable<Cart>;
    total$: Observable<number>;

    constructor(private store: Store<any>) {
        this.cart$ = this.store.select<Cart>(state => state.user.userState.cart);
    }

    ngOnInit() {

    }

    completeOrder(id: string) {
        this.store.dispatch(new userActions.CompleteOrderAction(id));
    }
}