import { UserState } from './user.state';
import { Action } from '@ngrx/store';
import * as userActions from './user.actions';
import { Result } from '../../models/result-vm';

export const initialState: UserState = {
    cart: undefined,
    user: undefined,
    redirectToLogin: false
};

export function userReducer(state = initialState, action: userActions.Actions): UserState {
    switch (action.type) {

        case userActions.GET_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.userCart !== null ? action.userCart.cart : undefined,
                user: action.userCart !== null ? action.userCart.user : undefined
            });

        case userActions.ADD_PRODUCT_TO_CART_COMPLETE:
        case userActions.REMOVE_PRODUCT_FROM_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.cart
            });

        case userActions.LOGIN_USER_COMPLETE:
            return Object.assign({}, state, {
                user: action.result.data
            });

        case userActions.REGISTER_USER_COMPLETE:
            return Object.assign({}, state, {
                redirectToLogin: action.result.result === Result.SUCCESS ? true: false
            });

        case userActions.COMPLETE_ORDER_COMPLETE:
            return Object.assign({}, state, {
                cart: undefined
            });

        default:
            return state;

    }
}
