import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Product } from "../../models/product";
import { ProductMedia } from "../../models/product-media";
import { ProductComponent } from "../../models/product-component";

import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';

@Component({
    selector: 'product-details-presentation',
    templateUrl: './details.html',
    styleUrls: ['./details.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class ProductDetailsPresentationComponent {
    _product: Product;
    _medias: ProductMedia[];
    _features: ProductComponent[];
    _specs: ProductComponent[];
    _compatibilies: ProductComponent[];
    _imaging: ProductComponent[];

    galleryOptions: NgxGalleryOptions[] = [
        {
            width: '600px',
            height: '400px',
            thumbnailsColumns: 4,
            imageAnimation: NgxGalleryAnimation.Slide
        },
        // max-width 800
        {
            breakpoint: 800,
            width: '100%',
            height: '600px',
            imagePercent: 80,
            thumbnailsPercent: 20,
            thumbnailsMargin: 20,
            thumbnailMargin: 20
        },
        // max-width 400
        {
            breakpoint: 400,
            preview: false
        }
    ];
    galleryImages: NgxGalleryImage[] = [];

    @Input()
    set product(value: Product) {
        this._product = Object.assign({}, value);

        if (this._product && this._product.components) {
            this._medias = [];
            this._features = [];
            this.galleryImages = [];
            this._product.components.forEach(component => {
                if (component.componentType === 'Media') {
                    component.medias.forEach(media => {
                        this._medias.push(media);
                        this.galleryImages.push({
                            small: media.url,
                            medium: media.url,
                            big: media.url
                        });
                    }
                    );

                }
                else if (component.componentType === 'Feature') {
                    this._features.push(component);
                }
                else if (component.componentType === 'Specification') {
                    this._specs.push(component);
                }
                else if (component.componentType === 'Compatibility') {
                    this._compatibilies.push(component);
                }
                else if (component.componentType === 'Imaging') {
                    this._imaging.push(component);
                }
            })
        }
    }

    get product() {
        return this._product;
    }

    @Output() addToCart: EventEmitter<string> = new EventEmitter();

    constructor() {
        this._medias = [];
        this._features = [];
        this._specs = [];
        this._compatibilies = [];
        this._imaging = [];
    }
    addProduct(productId: string) {
        this.addToCart.emit(productId);
    }

}