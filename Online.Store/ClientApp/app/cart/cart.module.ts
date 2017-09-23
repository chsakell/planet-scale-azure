import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { cartReducer } from './store/cart.reducer';
import { CartEffects } from './store/cart.effects';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        StoreModule.forFeature('basket', {
            cartState: cartReducer,
        }),
        EffectsModule.forFeature([CartEffects])
    ],
    exports: [],
    declarations: [],
    providers: [],
})
export class CartModule { }
