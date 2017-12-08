import { ForumState } from './forum.state';
import { Topic } from './../../models/topic';
import { Action } from '@ngrx/store';
import * as forumAction from './forum.actions';

export const initialState: ForumState = {
    topics: [],
    selectedTopic: undefined,
    previousContinuationToken: undefined,
    nextContinuationToken: undefined
};

export function forumReducer(state = initialState, action: forumAction.Actions): ForumState {
    switch (action.type) {

        case forumAction.SELECTALL_COMPLETE:
            return Object.assign({}, state, {
                topics: action.topics.topics,
                nextContinuationToken: action.topics.continuationToken
            });

        case forumAction.SET_PREVIOUS_TOKEN:
        return Object.assign({}, state, {
            previousContinuationToken: action.token,
        }); 

        case forumAction.SELECT_TOPIC_COMPLETE:
            return Object.assign({}, state, {
                selectedTopic: action.topic
            });

        default:
            return state;

    }
}
