import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ACCOUNT_ROUTES } from './account.routes';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { FormsModule } from '@angular/forms';
import { AccountRegisterComponent } from './register/register.component';
import { AccountRegisterPresentationComponent } from './register/register';
import { AccountLoginComponent } from './login/login.component';
import { AccountLoginPresentationComponent } from './login/login';
import { AccountComponent } from './account.component';
import { accountReducer } from './store/account.reducer';
import { AccountEffects } from './store/account.effects';

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
        StoreModule.forFeature('account', {
            accountState: accountReducer,
        }),
        EffectsModule.forFeature([AccountEffects])
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
