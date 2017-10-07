import { AccountState } from './account.state';
import { Action } from '@ngrx/store';
import * as accountActions from './account.actions';

export const initialState: AccountState = {
    newUser: { email: '', username: '', password: '', confirmPassword: '' },
    loginUser: { username: '', password: '', rememberMe: false }
};

export function accountReducer(state = initialState, action: accountActions.Actions): AccountState {
    switch (action.type) {

        case accountActions.REGISTER_USER_COMPLETE:
        case accountActions.SET_REGISTER_USER:
            return Object.assign({}, state, {
                newUser: action.user
            });

        case accountActions.LOGIN_USER_COMPLETE:
        case accountActions.SET_LOGIN_USER:
            return Object.assign({}, state, {
                loginUser: action.user
            });

        default:
            return state;

    }
}
