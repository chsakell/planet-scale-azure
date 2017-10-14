import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';

import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { of } from 'rxjs/observable/of';
import { Observable } from 'rxjs/Rx';

import * as accountActions from './account.actions';
import * as userActions from '../../user/store/user.actions';
import { AccountService } from '../../core/services/account.service';
import { Cart } from "../../models/cart";
import { RegisterVM } from '../../models/register-vm';
import { LoginVM } from '../../models/login-vm';
import { ResultVM } from '../../models/result-vm';

@Injectable()
export class AccountEffects {

    @Effect() registerUser: Observable<Action> = this.actions$.ofType(accountActions.REGISTER_USER)
        .switchMap((action: accountActions.RegisterUserAction) => {
            return this.accountService.registerUser(action.user)
                .map((data: RegisterVM) => {
                    return new accountActions.RegisterUserCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'registerUser_FAILED' })
                })
        }
        );

        @Effect() loginUser: Observable<Action> = this.actions$.ofType(accountActions.LOGIN_USER)
        .switchMap((action: accountActions.LoginUserAction) => {
            return this.accountService.loginUser(action.user)
                .map((data: ResultVM) => {
                    return new userActions.LoginUserCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'loginUser_FAILED' })
                })
        }
        );

    constructor(
        private accountService: AccountService,
        private actions$: Actions
    ) { }
}
