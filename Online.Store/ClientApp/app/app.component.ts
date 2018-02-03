import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router, Event } from '@angular/router';
import { Store } from '@ngrx/store';
import { UserState } from './user/store/user.state';
import * as userActions from './user/store/user.actions';
import { Observable } from 'rxjs/Observable';
import { Cart } from './models/cart';
import { User } from './models/user';
import { NotifyService } from './core/services/notifications.service';
import { Message, MessageType } from './models/message';
import { ISubscription } from 'rxjs/Subscription';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

    private subscription: ISubscription;

    cart$: Observable<Cart>;
    user$: Observable<User>;
    firstRouteEnd: boolean = false;

    public options = {
        position: ["bottom", "left"],
        timeOut: 5000,
        lastOnBottom: true,
        showProgressBar: true
    }

    public loading = false;

    constructor(private store: Store<any>, public notifyService: NotifyService, private router: Router) {
        this.cart$ = this.store.select<Cart>(state => state.user.userState.cart);
        this.user$ = this.store.select<User>(state => state.user.userState.user);
    }

    ngOnInit(): void {
        const self = this;

        this.router.events.subscribe((event: Event) => {
            if (event instanceof NavigationEnd) {
                if (!self.firstRouteEnd) {
                    self.firstRouteEnd = true;
                    self.store.dispatch(new userActions.GetCartAction());
                }
            }
        });

        this.subscription = this.notifyService.loading$.subscribe(loading => {
            setTimeout(function () {
                self.loading = loading;
            }, 100);
        });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

}
