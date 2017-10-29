import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { NavMenuComponent } from './navmenu/navmenu.component';
import { NavBarComponent } from './navmenu/navbar.component';
import { HomeComponent } from './home/home.component';
import { CheckoutComponent } from "./checkout/checkout.component";
import { AppRoutes } from './app.routes';
import { ProductModule } from './product/product.module';

import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';

import { reducers } from "./reducers";
import { CheckoutPresentationComponent } from "./checkout/checkout";

import { NgxGalleryModule } from 'ngx-gallery';
import { SimpleNotificationsModule } from 'angular2-notifications';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        NavBarComponent,
        CheckoutComponent,
        CheckoutPresentationComponent,
        HomeComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        AppRoutes,
        CoreModule.forRoot(),
        NgxGalleryModule,
        SimpleNotificationsModule.forRoot(),
        StoreModule.forRoot(reducers),
        StoreDevtoolsModule.instrument({
            maxAge: 5 //  Retains last 25 states
        }),
        EffectsModule.forRoot([])
    ]
}) 
export class AppModuleShared {
}
