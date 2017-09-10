import { Action } from '@ngrx/store';
import { Product } from './../../models/product';

export const SELECTALL = '[Product] Select All';
export const SELECTALL_COMPLETE = '[Product] Select All Complete';

export class SelectAllAction implements Action {
    readonly type = SELECTALL;

    constructor() { }
}

export class SelectAllCompleteAction implements Action {
    readonly type = SELECTALL_COMPLETE;

    constructor(public products: Product[]) { }
}

export type Actions
    = SelectAllAction
    | SelectAllCompleteAction;

