import { Action } from '@ngrx/store';
import { Product } from './../../models/product';
import { Cart } from "../../models/cart";

export const SELECTALL = '[Product] Select All';
export const SELECTALL_COMPLETE = '[Product] Select All Complete';
export const SELECT_PRODUCT = '[Product] Select Product';
export const SELECT_PRODUCT_COMPLETE = '[Product] Select Product Complete';
export const ADD_PRODUCT_TO_CART = '[Cart] Add Product To Cart';
export const ADD_PRODUCT_TO_CART_COMPLETE = '[Cart] Add Product To Cart Complete';
export const GET_CART = '[Cart] Get';
export const GET_CART_COMPLETE = '[Cart] Get Complete';

export class SelectAllAction implements Action {
    readonly type = SELECTALL;

    constructor() { }
}

export class SelectAllCompleteAction implements Action {
    readonly type = SELECTALL_COMPLETE;

    constructor(public products: Product[]) { }
}

export class SelectProductAction implements Action {
    readonly type = SELECT_PRODUCT;

    constructor(public id: string) { }
}

export class SelectProductCompleteAction implements Action {
    readonly type = SELECT_PRODUCT_COMPLETE;

    constructor(public product: Product) { }
}

export class AddProductToCartAction implements Action {
    readonly type = ADD_PRODUCT_TO_CART;

    constructor(public id: string) { }
}

export class AddProductToCartCompleteAction implements Action {
    readonly type = ADD_PRODUCT_TO_CART_COMPLETE;

    constructor(public cart: Cart) { }
}

export class GetCartAction implements Action {
    readonly type = GET_CART;

    constructor() { }
}

export class GetCartCompleteAction implements Action {
    readonly type = GET_CART_COMPLETE;

    constructor(public cart: Cart) { }
}

export type Actions
    = SelectAllAction
    | SelectAllCompleteAction
    | SelectProductAction
    | SelectProductCompleteAction
    | AddProductToCartAction
    | AddProductToCartCompleteAction
    | GetCartAction
    | GetCartCompleteAction;

