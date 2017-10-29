import 'rxjs/add/operator/map';

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

// Import RxJs required methods
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { NotificationsService } from 'angular2-notifications';
import { Store } from '@ngrx/store';
import { ISubscription } from 'rxjs/Subscription';
import * as notifyActions from '../../notifications/store/notifications.actions';

@Injectable()
export class NotifyService {

    loading$: Observable<boolean>;
    private subscription: ISubscription;

    constructor(private store: Store<any>, private _service: NotificationsService) {
        this.loading$ = this.store.select<boolean>(state => state.notifications.notificationsState.loading);
    }

    ngOnInit() {
        /*
        this.subscription = this.redirectToLogin$
            .filter(val => val === true)
            .subscribe(() => window.location.href = '/account/signin');
        */
    }

    ngOnDestroy() {
        // this.subscription.unsubscribe();
    }

    setLoading(loading: boolean) {
        this.store.dispatch(new notifyActions.SetLoadingAction(loading));
    }

}
