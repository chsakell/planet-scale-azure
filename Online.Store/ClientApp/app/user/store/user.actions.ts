import { Action } from '@ngrx/store';
import { Cart } from "../../models/cart";
import { LoginVM } from '../../models/login-vm';
import { ResultVM } from '../../models/result-vm';

export const LOGIN_USER = '[Account] Login User';
export const GET_CART = '[Cart] Get';
export const GET_CART_COMPLETE = '[Cart] Get Complete';
export const ADD_PRODUCT_TO_CART = '[Cart] Add Product To Cart';
export const ADD_PRODUCT_TO_CART_COMPLETE = '[Cart] Add Product To Cart Complete';
export const COMPLETE_ORDER = '[Order] Complete';
export const COMPLETE_ORDER_COMPLETE = '[Order] Completed';
export const LOGIN_USER_COMPLETE = '[Account] Login User Complete';


export class LoginUserAction implements Action {
    readonly type = LOGIN_USER;

    constructor(public user: LoginVM) { }
}

export class GetCartAction implements Action {
    readonly type = GET_CART;

    constructor() { }
}

export class GetCartCompleteAction implements Action {
    readonly type = GET_CART_COMPLETE;

    constructor(public cart: Cart) { }
}

export class AddProductToCartAction implements Action {
    readonly type = ADD_PRODUCT_TO_CART;

    constructor(public id: string) { }
}

export class AddProductToCartCompleteAction implements Action {
    readonly type = ADD_PRODUCT_TO_CART_COMPLETE;

    constructor(public cart: Cart) { }
}

export class CompleteOrderAction implements Action {
    readonly type = COMPLETE_ORDER;

    constructor(public id: string) { }
}

export class CompleteOrderCompleteAction implements Action {
    readonly type = COMPLETE_ORDER_COMPLETE;

    constructor(public id: string) { }
}

export class LoginUserCompleteAction implements Action {
    readonly type = LOGIN_USER_COMPLETE;

    constructor(public result: ResultVM) { }
}

export type Actions
    = GetCartAction
    | GetCartCompleteAction
    | AddProductToCartAction
    | AddProductToCartCompleteAction
    | LoginUserAction
    | LoginUserCompleteAction;

