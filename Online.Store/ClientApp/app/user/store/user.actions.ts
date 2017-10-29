import { Action } from '@ngrx/store';
import { Cart } from "../../models/cart";
import { LoginVM } from '../../models/login-vm';
import { ResultVM } from '../../models/result-vm';
import { RegisterVM } from '../../models/register-vm';
import { UserCart } from '../../models/user-cart';

export const GET_CART = '[Cart] Get';
export const GET_CART_COMPLETE = '[Cart] Get Complete';
export const ADD_PRODUCT_TO_CART = '[Cart] Add Product To Cart';
export const ADD_PRODUCT_TO_CART_COMPLETE = '[Cart] Add Product To Cart Complete';
export const REMOVE_PRODUCT_FROM_CART = '[Cart] Remove Product From Cart';
export const REMOVE_PRODUCT_FROM_CART_COMPLETE = '[Cart] Remove Product From Cart Complete';
export const COMPLETE_ORDER = '[Order] Complete';
export const COMPLETE_ORDER_COMPLETE = '[Order] Completed';
export const LOGIN_USER = '[Account] Login User';
export const LOGIN_USER_COMPLETE = '[Account] Login User Complete';
export const REGISTER_USER = '[Account] Register User';
export const REGISTER_USER_COMPLETE = '[Account] Register User Complete';

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

    constructor(public userCart: UserCart) { }
}

export class AddProductToCartAction implements Action {
    readonly type = ADD_PRODUCT_TO_CART;

    constructor(public id: string) { }
}

export class AddProductToCartCompleteAction implements Action {
    readonly type = ADD_PRODUCT_TO_CART_COMPLETE;

    constructor(public cart: Cart) { }
}

export class RemoveProductFromCartAction implements Action {
    readonly type = REMOVE_PRODUCT_FROM_CART;

    constructor(public id: string) { }
}

export class RemoveProductFromCartCompleteAction implements Action {
    readonly type = REMOVE_PRODUCT_FROM_CART_COMPLETE;

    constructor(public cart: Cart) { }
}

export class CompleteOrderAction implements Action {
    readonly type = COMPLETE_ORDER;

    constructor(public id: string) { }
}

export class CompleteOrderCompleteAction implements Action {
    readonly type = COMPLETE_ORDER_COMPLETE;

    constructor(public id: ResultVM) { }
}

export class RegisterUserAction implements Action {
    readonly type = REGISTER_USER;

    constructor(public user: RegisterVM) { }
}

export class RegisterUserCompleteAction implements Action {
    readonly type = REGISTER_USER_COMPLETE;

    constructor(public result: ResultVM) { }
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
    | RemoveProductFromCartAction
    | RemoveProductFromCartCompleteAction
    | LoginUserAction
    | LoginUserCompleteAction
    | RegisterUserAction
    | RegisterUserCompleteAction
    | CompleteOrderCompleteAction;

