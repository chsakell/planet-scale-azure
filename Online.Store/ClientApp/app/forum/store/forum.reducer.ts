import { ForumState } from './forum.state';
import { Topic } from './../../models/topic';
import { Action } from '@ngrx/store';
import * as forumAction from './forum.actions';

export const initialState: ForumState = {
    topics: new Map<number, Topic[]>(),
    selectedPage: 0,
    continuationToken: undefined,
    selectedTopic: undefined
};

export function forumReducer(state = initialState, action: forumAction.Actions): ForumState {
    switch (action.type) {

        case forumAction.SELECTALL_COMPLETE:
        let currentMap = new Map(state.topics);
        currentMap.set(state.selectedPage + 1, action.topics.topics);
            return Object.assign({}, state, {
                selectedPage: state.selectedPage + 1,
                continuationToken: action.topics.continuationToken,
                topics: currentMap
            });

        case forumAction.SELECT_TOPIC_COMPLETE:
            return Object.assign({}, state, {
                selectedTopic: action.topic
            });

        case forumAction.SET_SELECTED_PAGE:
        return Object.assign({}, state, {
            selectedPage: action.page
        }); 

        default:
            return state;

    }
}
