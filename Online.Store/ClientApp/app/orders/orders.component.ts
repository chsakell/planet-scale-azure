import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
//import { ForumState } from '../store/forum.state';
import { Observable } from 'rxjs/Observable';
import * as orderActions from '../user/store/user.actions';
import { Order } from '../models/order';
import { User } from '../models/user';
import { ISubscription } from 'rxjs/Subscription';
//import { Topic } from "../../models/topic";

@Component({
    selector: 'orders',
    templateUrl: './orders.component.html'
})

export class OrdersComponent implements OnInit {

    private subscription: ISubscription;
    orders$: Observable<Order[]>;
    user$: Observable<User>;

    constructor(private store: Store<any>) {
        this.orders$ = this.store.select<Order[]>(state => state.user.userState.orders);
        this.user$ = this.store.select<User>(state => state.user.userState.user);
    }

    ngOnInit() {
        const self = this;

        self.subscription = self.user$
            .filter(user => user !== null && user !== undefined)
            .subscribe(user => self.store.dispatch(new orderActions.GetOrdersAction(user.id)));
    }

    ngOnDestroy() {
        if (this.subscription)
            this.subscription.unsubscribe();
    }
}