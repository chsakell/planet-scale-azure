import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Product } from "../../models/product";

@Component({
    selector: 'product-list-presentation',
    templateUrl: './list.html',
    styleUrls: ['./list.css'],
    changeDetection: ChangeDetectionStrategy.OnPush,
})

export class ProductListPresentationComponent {

    @Input() products: Product[];
    @Input() loading: boolean;
    @Output() addToCart: EventEmitter<string> = new EventEmitter();

    constructor() { }

    addProduct(productId: string) {
        this.addToCart.emit(productId);
    }
}

