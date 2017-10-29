import { ActionReducerMap } from '@ngrx/store';

import { userReducer } from '../user/store/user.reducer';
import { UserState } from '../user/store/user.state';

import { notificationsReducer } from '../notifications/store/notifications.reducer';
import { NotificationsState } from '../notifications/store/notifications.state';

/**
 * As mentioned, we treat each reducer like a table in a database. This means
 * our top level state interface is just a map of keys to inner state types.
 */
export interface State {
    user: UserState;
    notification: NotificationsState
}

/**
 * Our state is composed of a map of action reducer functions.
 * These reducer functions are called with each dispatched action
 * and the current or initial state and return a new immutable state.
 */
export const reducers: ActionReducerMap<State> = {
    user: userReducer,
    notification: notificationsReducer
};