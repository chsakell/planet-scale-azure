import { Action } from '@ngrx/store';
import { RegisterVM } from '../../models/register-vm';
import { LoginVM } from '../../models/login-vm';

export const SET_REGISTER_USER = '[Account] Set Register User';
export const REGISTER_USER = '[Account] Register User';
export const REGISTER_USER_COMPLETE = '[Account] Register User Complete';
export const SET_LOGIN_USER = '[Account] Set Login User';
export const LOGIN_USER = '[Account] Login User';
export const LOGIN_USER_COMPLETE = '[Account] Login User Complete';

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

export class LoginUserAction implements Action {
    readonly type = LOGIN_USER;

    constructor(public user: LoginVM) { }
}

export class LoginUserCompleteAction implements Action {
    readonly type = LOGIN_USER_COMPLETE;

    constructor(public user: LoginVM) { }
}

export class SetLoginUserAction implements Action {
    readonly type = SET_LOGIN_USER;

    constructor(public user: LoginVM) { }
}

export type Actions
    = SetRegisterUserAction
    | RegisterUserAction
    | RegisterUserCompleteAction
    | SetLoginUserAction
    | LoginUserAction
    | LoginUserCompleteAction;

