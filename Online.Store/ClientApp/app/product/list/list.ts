import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Product } from "../../models/product";

@Component({
    selector: 'product-list-presentation',
    templateUrl: './list.html'
})

export class ProductListPresentationComponent {

    @Input() products: Product[];

    constructor() { }

}