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
import { Message, MessageType } from '../../models/message';

@Injectable()
export class NotifyService {

    loading$: Observable<boolean>;
    message$: Observable<Message>;
    private subscription: ISubscription;

    constructor(private store: Store<any>, private _service: NotificationsService) {
        this.loading$ = this.store.select<boolean>(state => state.notifications.notificationsState.loading);
        this.message$ = this.store.select<Message>(state => state.notifications.notificationsState.message);

        this.message$
            .skip(1)
            .filter(message => message.message !== null && message.message !== undefined)
            .subscribe((notification) => {
                switch (notification.type) {
                    case MessageType.SUCCESS:
                        this._service.success('Success', notification.message);
                        break;
                    case MessageType.Error:
                        this._service.error('Error', notification.message);
                        break;
                    default:
                        this._service.info('Info', notification.message);
                        break;
                }
            });
    }

    setLoading(loading: boolean) {
        this.store.dispatch(new notifyActions.SetLoadingAction(loading));
    }

    setMessage(message: Message) {
        this.store.dispatch(new notifyActions.SetMessageAction(message));
    }

}
