import { Action } from '@ngrx/store';
import { Message } from '../../models/message';

export const SET_LOADING = '[Notify] Set Loading';
export const SET_MESSAGE = '[Notify] Set Message';

export class SetLoadingAction implements Action {
    readonly type = SET_LOADING;

    constructor(public loading: boolean) { }
}

export class SetMessageAction implements Action {
    readonly type = SET_MESSAGE;

    constructor(public message: Message) { }
}

export type Actions
    = SetLoadingAction
    | SetMessageAction;

