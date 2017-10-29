import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import * as userActions from '../../user/store/user.actions';
import { Topic } from "../../models/topic";
import { RegisterVM } from '../../models/register-vm';
import { ISubscription } from "rxjs/Subscription";

@Component({
    selector: 'user-register',
    templateUrl: './register.component.html',

})

export class UserRegisterComponent implements OnInit {

    newUser$: Observable<RegisterVM>;
    private subscription: ISubscription;

    constructor(private store: Store<any>) {
    }

    ngOnInit() { 
    }

    registerUser(user: RegisterVM) {
        this.store.dispatch(new userActions.RegisterUserAction(user));
    }

    ngOnDestroy() {
        //this.subscription.unsubscribe();
    }
}