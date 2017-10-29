import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { userReducer } from './store/user.reducer';
import { UserEffects } from './store/user.effects';
import { UserRegisterPresentationComponent } from './register/register';
import { UserRegisterComponent } from './register/register.component';
import { UserComponent } from './user.component';

const USER_DIRECTIVES = [
    UserComponent,
    UserRegisterComponent,
    UserRegisterPresentationComponent
];

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        StoreModule.forFeature('user', {
            userState: userReducer,
        }),
        EffectsModule.forFeature([UserEffects])
    ],
    exports: [
        ...USER_DIRECTIVES
    ],
    declarations: [
        ...USER_DIRECTIVES
    ],
    providers: [],
})
export class UserModule { }
