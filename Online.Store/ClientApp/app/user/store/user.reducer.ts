import { UserState } from './user.state';
import { Action } from '@ngrx/store';
import * as userActions from './user.actions';
import { Result } from '../../models/result-vm';
import { CartItem } from '../../models/cart-item';

export const initialState: UserState = {
    cart: undefined,
    cartTotal: 0,
    user: undefined,
    redirectToLogin: false
};

export function userReducer(state = initialState, action: userActions.Actions): UserState {
    switch (action.type) {

        case userActions.GET_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.userCart !== null ? action.userCart.cart : undefined,
                cartTotal: calculateTotal(action.userCart.cart),
                user: action.userCart !== null ? action.userCart.user : undefined
            });

        case userActions.ADD_PRODUCT_TO_CART_COMPLETE:
        case userActions.REMOVE_PRODUCT_FROM_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.cart,
                cartTotal: calculateTotal(action.cart)
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
                cart: undefined,
                calculateTotal: 0
            });

        default:
            return state;

    }
}

function calculateTotal(cart: any) {
    let total = 0;

    if(cart !== null && cart !== undefined) {
        if(cart.items !== undefined && cart.items.length > 0) {
            cart.items.forEach((item: CartItem) => {
                total+= item.price * item.quantity;
            });
        }
    }

    return total;
}
