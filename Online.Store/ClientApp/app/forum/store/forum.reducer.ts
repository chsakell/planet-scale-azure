import { ForumState } from './forum.state';
import { Topic } from './../../models/topic';
import { Action } from '@ngrx/store';
import * as forumAction from './forum.actions';

export const initialState: ForumState = {
    topics: [],
    selectedTopic: undefined
};

export function forumReducer(state = initialState, action: forumAction.Actions): ForumState {
    switch (action.type) {

        case forumAction.SELECTALL_COMPLETE:
            return Object.assign({}, state, {
                topics: action.topics
            });

        case forumAction.SELECT_TOPIC_COMPLETE:
            return Object.assign({}, state, {
                selectedTopic: action.topic
            });

        case forumAction.ADD_REPLY_COMPLETE:
            return Object.assign({}, state, {
                selectedTopic: action.result.data
            });

        default:
            return state;

    }
}
