import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Cart } from "../models/cart";
import { User } from '../models/user';

@Component({
    selector: 'checkout-presentation',
    templateUrl: './checkout.html',
    styleUrls: ['./checkout.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class CheckoutPresentationComponent {
    _cart: Cart;
    
    @Input() total: number;
    @Input() user: User;

    @Input()
    set cart(value: Cart) {
        this._cart = Object.assign({}, value);
    }

    get cart() {
        return this._cart;
    }

    @Output() checkout: EventEmitter<string> = new EventEmitter();
    @Output() onRemoveProduct: EventEmitter<string> = new EventEmitter();

    constructor() { }

    completeOrder() {
        console.log(this.cart.id);
        this.checkout.emit(this.cart.id);
    }

    remove(id: string) {
        this.onRemoveProduct.emit(id);
    }

}