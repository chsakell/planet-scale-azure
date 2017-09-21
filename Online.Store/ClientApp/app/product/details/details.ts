import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Product } from "../../models/product";
import { ProductMedia } from "../../models/product-media";

@Component({
    selector: 'product-details-presentation',
    templateUrl: './details.html',
    styleUrls: ['./details.css'],
})

export class ProductDetailsPresentationComponent {
    _product: Product;
    _medias: ProductMedia[];

    @Input()
    set product(value: Product) {
        this._product = value;

        if (value) {
            value.components.forEach(component => {
                if (component.componentType === 'Media') {
                    component.medias.forEach(media => this._medias.push(media));
                }
            })
        }
    }

    get product() {
        return this._product;
    }

    constructor() {
        this._medias = [];
    }

}