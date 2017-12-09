import { Action } from '@ngrx/store';
import { Topic } from './../../models/topic';
import { Reply } from '../../models/reply';
import { ResultVM } from '../../models/result-vm';
import { PagedTopics } from '../../models/paged-topics';

export const SELECTALL = '[Forum] Select All Topics';
export const SELECTALL_COMPLETE = '[Forum] Select All Topics Complete';
export const SET_SELECTED_PAGE = '[Forum] Set Selected Page';
export const SELECT_TOPIC = '[Forum] Select Topic';
export const SELECT_TOPIC_COMPLETE = '[Forum] Select Topic Complete';
export const ADD_REPLY = '[Forum] Add Reply';
export const ADD_TOPIC = '[Forum] Add Topic';

export class SelectAllAction implements Action {
    readonly type = SELECTALL;

    constructor(public token?: string) { }
}

export class SelectAllCompleteAction implements Action {
    readonly type = SELECTALL_COMPLETE;

    constructor(public topics: PagedTopics) { }
}

export class SetSelectedPageAction implements Action {
    readonly type = SET_SELECTED_PAGE;

    constructor(public page: number) { }
}

export class SelectTopicAction implements Action {
    readonly type = SELECT_TOPIC;

    constructor(public id: string) { }
}

export class SelectTopicCompleteAction implements Action {
    readonly type = SELECT_TOPIC_COMPLETE;

    constructor(public topic: Topic) { }
}

export class AddReplyAction implements Action {
    readonly type = ADD_REPLY;

    constructor(public reply: Reply) {  }
}

export class AddTopicAction implements Action {
    readonly type = ADD_TOPIC;

    constructor(public topic: Reply) {  }
}

export type Actions
    = SelectAllAction
    | SelectAllCompleteAction
    | SetSelectedPageAction
    | SelectTopicAction
    | SelectTopicCompleteAction
    | AddReplyAction
    | AddTopicAction;

