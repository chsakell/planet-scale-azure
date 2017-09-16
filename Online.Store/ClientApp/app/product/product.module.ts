import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductListComponent } from './list/product-list.component';
import { ProductDetailsComponent } from './details/product-details';
import { PRODUCT_ROUTES } from './product.routes';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { productReducer } from './store/product.reducer';
import { ProductEffects } from './store/product.effects';
import { FormsModule } from '@angular/forms';
import { ProductComponent } from "./product.component";
import { ProductListPresentationComponent } from './list/list';

const PRODUCT_DIRECTIVES = [
    ProductListComponent,
    ProductListPresentationComponent,
    ProductDetailsComponent,
    ProductComponent
];

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        PRODUCT_ROUTES,
        StoreModule.forFeature('inventory', {
            productState: productReducer,
        }),
        EffectsModule.forFeature([ProductEffects])
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
