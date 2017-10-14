import { UserState } from './user.state';
import { Action } from '@ngrx/store';
import * as userActions from './user.actions';

export const initialState: UserState = {
    cart: undefined,
    user: undefined,
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

        default:
            return state;

    }
}
