import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ACCOUNT_ROUTES } from './account.routes';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
//import { forumReducer } from './store/forum.reducer';
//import { ForumEffects } from './store/forum.effects';
import { FormsModule } from '@angular/forms';
import { AccountRegisterComponent } from './register/register.component';
import { AccountRegisterPresentationComponent } from './register/register';
import { AccountLoginComponent } from './login/login.component';
import { AccountLoginPresentationComponent } from './login/login';
import { AccountComponent } from './account.component';


const ACCOUNT_DIRECTIVES = [
    AccountRegisterComponent,
    AccountRegisterPresentationComponent,
    AccountLoginComponent,
    AccountLoginPresentationComponent,
    AccountComponent
];

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ACCOUNT_ROUTES,
        /*
        StoreModule.forFeature('community', {
            forumState: forumReducer,
        }),
        EffectsModule.forFeature([ForumEffects])
        */
    ],
    exports: [
        ...ACCOUNT_DIRECTIVES
    ],
    declarations: [
        ...ACCOUNT_DIRECTIVES
    ],
    providers: [],
})
export class AccountModule { }
