import { Action } from '@ngrx/store';

export const SET_LOADING = '[Notify] Set Loading';
export const SET_MESSAGE = '[Notify] Set Message';

export class SetLoadingAction implements Action {
    readonly type = SET_LOADING;

    constructor(public loading: boolean) { }
}

export class SetMessageAction implements Action {
    readonly type = SET_MESSAGE;

    constructor(public message: string) { }
}

export type Actions
    = SetLoadingAction
    | SetMessageAction;

