import { Action } from '@ngrx/store';
import { RegisterVM } from '../../models/register-vm';

export const SET_REGISTER_USER = '[Account] Set Register User';
export const REGISTER_USER = '[Account] Register User';
export const REGISTER_USER_COMPLETE = '[Account] Register User Complete';

export class RegisterUserAction implements Action {
    readonly type = REGISTER_USER;

    constructor(public user: RegisterVM) { }
}

export class RegisterUserCompleteAction implements Action {
    readonly type = REGISTER_USER_COMPLETE;

    constructor(public user: RegisterVM) { }
}

export class SetRegisterUserAction implements Action {
    readonly type = SET_REGISTER_USER;

    constructor(public user: RegisterVM) { }
}

export type Actions
    = SetRegisterUserAction
    | RegisterUserAction
    | RegisterUserCompleteAction;

