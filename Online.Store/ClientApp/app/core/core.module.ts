import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';

import { Configuration } from '../app.constants';
import { ProductService } from './services/product.service';
import { UserModule } from "../user/user.module";
import { AccountService } from './services/account.service';
import { NotificationsModule } from '../notifications/notifications.module';
import { ForumService } from './services/forum.service';

@NgModule({
    imports: [
        CommonModule,
        UserModule,
    ],
    exports: [
        CommonModule,
        UserModule,
        NotificationsModule,
    ]
})

export class CoreModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: CoreModule,
            providers: [
                ProductService,
                AccountService,
                ForumService,
                Configuration
            ]
        };
    }
}
