import { ProductState } from './product.state';
import { Product } from './../../models/product';
import { Action } from '@ngrx/store';
import * as productsAction from './product.action';

export const initialState: ProductState = {
    products: [
        {
            productId: 'productid',
            productPrice: 3,
            productHeading: 'test',
            description: 'hello world',
            productURL: 'product url',
            productComponentsList: []
        }
    ]
};

export function productReducer(state = initialState, action: productsAction.Actions): ProductState {
    switch (action.type) {

        case productsAction.SELECTALL_COMPLETE:
            return Object.assign({}, state, {
                products: action.products
            });

        default:
            return state;

    }
}
