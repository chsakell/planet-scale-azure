import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';

import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { of } from 'rxjs/observable/of';
import { Observable } from 'rxjs/Rx';

import * as accountActions from './account.actions';
import { AccountService } from '../../core/services/account.service';
import { Cart } from "../../models/cart";
import { RegisterVM } from '../../models/register-vm';

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

    constructor(
        private accountService: AccountService,
        private actions$: Actions
    ) { }
}
