import { UserState } from './user.state';
import { Action } from '@ngrx/store';
import * as userActions from './user.actions';

export const initialState: UserState = {
    cart: undefined,
    user: undefined,
    selectedPanel: 'Login'
};

export function userReducer(state = initialState, action: userActions.Actions): UserState {
    switch (action.type) {

        case userActions.GET_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.cart
            });

        case userActions.ADD_PRODUCT_TO_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.cart
            });

        case userActions.LOGIN_USER_COMPLETE:
            return Object.assign({}, state, {
                user: action.result.data
            });

        case userActions.REGISTER_USER_COMPLETE:
            return Object.assign({}, state, {
                user: action.result.data
            });

        case userActions.SWITCH_ACCOUNT_ACTION:
            return Object.assign({}, state, {
                selectedPanel: action.view
            });

        default:
            return state;

    }
}
