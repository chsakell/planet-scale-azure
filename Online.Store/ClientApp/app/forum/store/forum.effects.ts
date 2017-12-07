import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';

import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { of } from 'rxjs/observable/of';
import { Observable } from 'rxjs/Rx';

import * as forumActions from './forum.actions';
import * as notifyActions from '../../notifications/store/notifications.actions';
import { Topic } from './../../models/topic';
import { ProductService } from '../../core/services/product.service';
import { ForumService } from '../../core/services/forum.service';
import { ResultVM } from '../../models/result-vm';
import { MessageType } from '../../models/message';
import { PagedTopics } from '../../models/paged-topics';

@Injectable()
export class ForumEffects {

    @Effect() getTopics$: Observable<Action> = this.actions$.ofType(forumActions.SELECTALL)
        .switchMap(() =>
            this.productService.getTopics()
                .mergeMap((data: PagedTopics) => {
                    return [
                        new notifyActions.SetLoadingAction(false),
                        new forumActions.SelectAllCompleteAction(data)
                    ];
                })
                .catch((error: any) => {
                    return of({ type: 'getTopics_FAILED' })
                })
        );

    @Effect() getTopicDetails$: Observable<Action> = this.actions$.ofType(forumActions.SELECT_TOPIC)
        .switchMap((action: forumActions.SelectTopicAction) => {
            return this.productService.getTopicDetails(action.id)
                .mergeMap((data: Topic) => {
                    return [
                        new notifyActions.SetLoadingAction(false),
                        new forumActions.SelectTopicCompleteAction(data)
                    ]
                })
                .catch((error: any) => {
                    return of({ type: 'getTopicDetails_FAILED' })
                })
        }
    );

    @Effect() addReply: Observable<Action> = this.actions$.ofType(forumActions.ADD_REPLY)
    .switchMap((action: forumActions.AddReplyAction) => {
        return this.forumService.addReply(action.reply)
            .mergeMap((result: ResultVM) => {
                return [
                    new notifyActions.SetLoadingAction(false),
                    new notifyActions.SetMessageAction( { type: MessageType.SUCCESS, message: 'Reply submitted successfully' }),
                    new forumActions.SelectTopicCompleteAction(result.data)
                ];
            })
            .catch((error: any) => {
                return of(new notifyActions.SetMessageAction( { type: MessageType.Error, message: 'Failed to submit reply' }))
            })
    }
    );

    @Effect() addTopic: Observable<Action> = this.actions$.ofType(forumActions.ADD_TOPIC)
    .switchMap((action: forumActions.AddTopicAction) => {
        return this.forumService.createTopic(action.topic)
            .mergeMap((result: ResultVM) => {
                return [
                    new notifyActions.SetLoadingAction(false),
                    new notifyActions.SetMessageAction( { type: MessageType.SUCCESS, message: 'Topic submitted successfully' }),
                    new forumActions.SelectAllAction()
                ];
            })
            .catch((error: any) => {
                return of(new notifyActions.SetMessageAction( { type: MessageType.Error, message: 'Failed to submit reply' }))
            })
    }
    );

    constructor(
        private productService: ProductService,
        private forumService: ForumService,
        private actions$: Actions
    ) { }
}
