import { Action } from '@ngrx/store';
import { Product } from './../../models/product';

export const SELECTALL = '[Product] Select All';
export const SELECTALL_COMPLETE = '[Product] Select All Complete';
export const SELECT_PRODUCT = '[Product] Select Product';
export const SELECT_PRODUCT_COMPLETE = '[Product] Select Product Complete';

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

export type Actions
    = SelectAllAction
    | SelectAllCompleteAction
    | SelectProductAction
    | SelectProductCompleteAction;

