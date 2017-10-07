import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import * as accountActions from '../store/account.actions';
import { LoginVM } from '../../models/login-vm';
import { ISubscription } from "rxjs/Subscription";

@Component({
    selector: 'account-login',
    templateUrl: './login.component.html',

})

export class AccountLoginComponent implements OnInit {

    loginUser$: Observable<LoginVM>;
    private subscription: ISubscription;

    constructor(private store: Store<any>) {
        this.loginUser$ = this.store.select<LoginVM>(state => state.account.accountState.loginUser);
    }

    ngOnInit() {
        this.subscription = this.loginUser$
        .skip(1)
        .filter(u => u.username != '' && u.password != '')
        .distinctUntilChanged()
        .subscribe(user => this.store.dispatch(new accountActions.LoginUserAction(user)));
    }

    loginUser(user: LoginVM) {
        this.store.dispatch(new accountActions.SetLoginUserAction(user));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}