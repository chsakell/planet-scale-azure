import { CartState } from './cart.state';
import { Action } from '@ngrx/store';
import * as cartAction from './cart.action';

export const initialState: CartState = {
    cart: undefined,
    user: undefined,
};

export function cartReducer(state = initialState, action: cartAction.Actions): CartState {
    switch (action.type) {

        case cartAction.GET_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.cart
            });

        case cartAction.ADD_PRODUCT_TO_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.cart
            });

        case cartAction.LOGIN_USER_COMPLETE:
            return Object.assign({}, state, {
                user: action.result.data
            });

        default:
            return state;

    }
}
