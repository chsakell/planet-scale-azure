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

    constructor(private store: Store<any>) {
        this.cart$ = this.store.select<Cart>(state => state.user.userState.cart);
    }

    ngOnInit() {
        //this.store.dispatch(new ProductActions.SelectAllAction());
    }

    completeOrder(id: string) {
        this.store.dispatch(new userActions.CompleteOrderAction(id));
    }
}