import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import * as userActions from '../../user/store/user.actions';
import { LoginVM } from '../../models/login-vm';
import { ISubscription } from "rxjs/Subscription";

@Component({
    selector: 'user-login',
    templateUrl: './login.component.html',

})

export class UserLoginComponent implements OnInit {

    //loginUser$: Observable<LoginVM>;
    private subscription: ISubscription;

    constructor(private store: Store<any>) {
        //this.loginUser$ = this.store.select<LoginVM>(state => state.account.accountState.loginUser);
    }

    ngOnInit() {
        /*
        this.subscription = this.loginUser$
        .skip(1)
        .filter(u => u.username != '' && u.password != '')
        .distinctUntilChanged()
        .subscribe(user => this.store.dispatch(new accountActions.LoginUserAction(user)));
        */
    }

    loginUser(user: LoginVM) {
        this.store.dispatch(new userActions.LoginUserAction(user));
    }

    switchView(view: string) {
        this.store.dispatch(new userActions.SwitchAccountAction(view));
    }

    ngOnDestroy() {
        //this.subscription.unsubscribe();
    }
}