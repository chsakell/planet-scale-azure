import { NotificationsState } from './notifications.state';
import { Action } from '@ngrx/store';
import * as notifyActions from './notifications.actions';
import { Result } from '../../models/result-vm';

export const initialState: NotificationsState = {
    loading: false,
    message: undefined
};

export function notificationsReducer(state = initialState, action: notifyActions.Actions): NotificationsState {
    switch (action.type) {

        case notifyActions.SET_LOADING:
            return Object.assign({}, state, {
                loading: action.loading
            });

        case notifyActions.SET_MESSAGE:
            return Object.assign({}, state, {
                message: action.message
            });

        default:
            return state;

    }
}
