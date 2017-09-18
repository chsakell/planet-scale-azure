import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Product } from "../../models/product";

@Component({
    selector: 'product-details-presentation',
    templateUrl: './details.html',
    styleUrls: ['./details.css'],
})

export class ProductDetailsPresentationComponent {

    @Input() product: Product;

    constructor() { }

}