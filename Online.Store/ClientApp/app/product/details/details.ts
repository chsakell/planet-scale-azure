import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Product } from "../../models/product";
import { ProductMedia } from "../../models/product-media";
import { ProductComponent } from "../../models/product-component";

@Component({
    selector: 'product-details-presentation',
    templateUrl: './details.html',
    styleUrls: ['./details.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class ProductDetailsPresentationComponent {
    _product: Product;
    _medias: ProductMedia[] = [];
    _features: ProductComponent[] = [];
    _specs: ProductComponent[] = [];
    _compatibilites: ProductComponent[] = [];
    _imaging: ProductComponent[] = [];
    cdnImage: string = '';
    originalImage: string;

    @Input() loading: boolean;
    @Input()
    set product(value: Product) {
        this._product = Object.assign({}, value);

        this.cdnImage = this._product.cdnImage;
        this.originalImage = this._product.image;

        if (this._product && this._product.components) {
            this._medias = [];
            this._features = [];
            this._product.components.forEach(component => {
                if (component.componentType === 'Media') {
                    component.medias.forEach(media => {
                        this._medias.push(media);
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
                    this._compatibilites.push(component);
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

    addProduct(productId: string) {
        this.addToCart.emit(productId);
    }

    setImages(cdnImage: string, originalImage: string) {
        this.cdnImage = cdnImage;
        this.originalImage = originalImage;
    }

}