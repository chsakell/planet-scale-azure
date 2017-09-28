import { Action } from '@ngrx/store';
import { Topic } from './../../models/topic';

export const SELECTALL = '[Forum] Select All Topics';
export const SELECTALL_COMPLETE = '[Forum] Select All Topics Complete';
export const SELECT_TOPIC = '[Forum] Select Topic';
export const SELECT_TOPIC_COMPLETE = '[Forum] Select Topic Complete';


export class SelectAllAction implements Action {
    readonly type = SELECTALL;

    constructor() { }
}

export class SelectAllCompleteAction implements Action {
    readonly type = SELECTALL_COMPLETE;

    constructor(public topics: Topic[]) { }
}

export class SelectTopicAction implements Action {
    readonly type = SELECT_TOPIC;

    constructor(public id: string) { }
}

export class SelectTopicCompleteAction implements Action {
    readonly type = SELECT_TOPIC_COMPLETE;

    constructor(public topic: Topic) { }
}



export type Actions
    = SelectAllAction
    | SelectAllCompleteAction
    | SelectTopicAction
    | SelectTopicCompleteAction;

