import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Cart } from "../models/cart";

@Component({
    selector: 'checkout-presentation',
    templateUrl: './checkout.html',
    styleUrls: ['./checkout.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class CheckoutPresentationComponent {
    _cart: Cart;

    @Input()
    set cart(value: Cart) {
        this._cart = Object.assign({}, value);
    }

    get cart() {
        return this._cart;
    }

    @Output() checkout: EventEmitter<string> = new EventEmitter();

    constructor() { }

    completeOrder(id: string) {
        this.checkout.emit(id);
    }

}