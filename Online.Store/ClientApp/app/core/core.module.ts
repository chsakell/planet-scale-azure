import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';

import { Configuration } from '../app.constants';
import { ProductService } from './services/product.service';
import { CartModule } from "../cart/cart.module";

@NgModule({
    imports: [
        CommonModule,
        CartModule
    ]
})

export class CoreModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: CoreModule,
            providers: [
                ProductService,
                Configuration
            ]
        };
    }
}
