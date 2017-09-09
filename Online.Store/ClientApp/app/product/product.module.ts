import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductListComponent } from './list/product-list.component';
import { ProductDetailsComponent } from './details/product-details';
import { PRODUCT_ROUTES } from './product.routes';

const PRODUCT_DIRECTIVES = [
    ProductListComponent,
    ProductDetailsComponent
];

@NgModule({
    imports: [
        PRODUCT_ROUTES
    ],
    exports: [
        ...PRODUCT_DIRECTIVES
    ],
    declarations: [
        ...PRODUCT_DIRECTIVES
    ],
    providers: [],
})
export class ProductModule { }
