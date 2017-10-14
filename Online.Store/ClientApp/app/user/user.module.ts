import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { userReducer } from './store/user.reducer';
import { UserEffects } from './store/user.effects';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        StoreModule.forFeature('user', {
            userState: userReducer,
        }),
        EffectsModule.forFeature([UserEffects])
    ],
    exports: [],
    declarations: [],
    providers: [],
})
export class UserModule { }
