import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import * as userActions from '../../user/store/user.actions';
import { Topic } from "../../models/topic";
import { RegisterVM } from '../../models/register-vm';
import { ISubscription } from "rxjs/Subscription";
import { LoginVM } from '../../models/login-vm';

@Component({
    selector: 'user-register',
    templateUrl: './register.component.html',

})

export class UserRegisterComponent implements OnInit {

    newUser$: Observable<RegisterVM>;
    useIdentity$: Observable<boolean>;
    private subscription: ISubscription;

    constructor(private store: Store<any>) {
        this.useIdentity$ = this.store.select<boolean>(state => state.user.userState.useIdentity);
    }

    ngOnInit() {
    }

    registerUser(user: RegisterVM) {
        this.useIdentity$.take(1).subscribe(useidentity => {
            this.store.dispatch(new userActions.RegisterUserAction(user));
        })
    }

    loginUser(user: LoginVM) {
        this.store.dispatch(new userActions.LoginUserAction(user));
    }

    ngOnDestroy() {
        //this.subscription.unsubscribe();
    }
}