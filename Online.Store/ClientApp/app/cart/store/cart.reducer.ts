import { CartState } from './cart.state';
import { Action } from '@ngrx/store';
import * as cartAction from './cart.action';

export const initialState: CartState = {
    cart: undefined
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

        default:
            return state;

    }
}
