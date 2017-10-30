import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Cart } from "../models/cart";
import * as userActions from '../user/store/user.actions';
import { NotifyService } from '../core/services/notifications.service';
import { Message, MessageType } from '../models/message';

@Component({
    selector: 'checkout-order',
    templateUrl: './checkout.component.html',

})

export class CheckoutComponent implements OnInit {

    cart$: Observable<Cart>;
    total$: Observable<number>;

    constructor(private store: Store<any>, private notifyService: NotifyService) {
        this.cart$ = this.store.select<Cart>(state => state.user.userState.cart);
    }

    ngOnInit() {

    }

    completeOrder(id: string) {
        this.displayMessage('Processing request..');
        this.store.dispatch(new userActions.CompleteOrderAction(id));
    }

    removeProduct(productId: string) {
        this.store.dispatch(new userActions.RemoveProductFromCartAction(productId));
    }

    displayMessage(message: string) {
        const notification: Message = { type: MessageType.Info, message: message } ;
        this.notifyService.setMessage(notification);
    }
}