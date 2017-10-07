import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import * as accountActions from '../store/account.actions';
import { Topic } from "../../models/topic";
import { RegisterVM } from '../../models/register-vm';
import { ISubscription } from "rxjs/Subscription";

@Component({
    selector: 'account-register',
    templateUrl: './register.component.html',

})

export class AccountRegisterComponent implements OnInit {

    newUser$: Observable<RegisterVM>;
    private subscription: ISubscription;

    constructor(private store: Store<any>) {
        this.newUser$ = this.store.select<RegisterVM>(state => state.account.accountState.newUser);
    }

    ngOnInit() {
        this.subscription = this.newUser$
        .skip(1)
        .filter(u => u.email != '' && u.username != '' && u.password != '' && u.confirmPassword != '')
        .distinctUntilChanged()
        .subscribe(user => this.store.dispatch(new accountActions.RegisterUserAction(user)));
    }

    registerUser(user: RegisterVM) {
        this.store.dispatch(new accountActions.SetRegisterUserAction(user));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}