import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';

import { Configuration } from '../app.constants';
import { ProductService } from './services/product.service';
import { UserModule } from "../user/user.module";
import { AccountService } from './services/account.service';

@NgModule({
    imports: [
        CommonModule,
        UserModule
    ]
})

export class CoreModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: CoreModule,
            providers: [
                ProductService,
                AccountService,
                Configuration
            ]
        };
    }
}
