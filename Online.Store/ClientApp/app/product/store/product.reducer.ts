import { ProductState } from './product.state';
import { Product } from './../../models/product';
import { Action } from '@ngrx/store';
import * as productsAction from './product.action';

export const initialState: ProductState = {
    products: [],
    selectedProduct: undefined,
    cart: undefined
};

export function productReducer(state = initialState, action: productsAction.Actions): ProductState {
    switch (action.type) {

        case productsAction.SELECTALL_COMPLETE:
            return Object.assign({}, state, {
                products: action.products
            });

        case productsAction.SELECT_PRODUCT_COMPLETE:
            return Object.assign({}, state, {
                selectedProduct: action.product
            });

        case productsAction.GET_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.cart
            });

        case productsAction.ADD_PRODUCT_TO_CART_COMPLETE:
            return Object.assign({}, state, {
                cart: action.cart
            });

        default:
            return state;

    }
}
