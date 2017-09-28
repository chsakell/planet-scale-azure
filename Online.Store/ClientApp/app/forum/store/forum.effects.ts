import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';

import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { of } from 'rxjs/observable/of';
import { Observable } from 'rxjs/Rx';

import * as forumAction from './forum.actions';
import { Topic } from './../../models/topic';
import { ProductService } from '../../core/services/product.service';

@Injectable()
export class ForumEffects {

    @Effect() getTopics$: Observable<Action> = this.actions$.ofType(forumAction.SELECTALL)
        .switchMap(() =>
            this.productService.getTopics()
                .map((data: Topic[]) => {
                    return new forumAction.SelectAllCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'getTopics_FAILED' })
                })
        );

    @Effect() getTopicDetails$: Observable<Action> = this.actions$.ofType(forumAction.SELECT_TOPIC)
        .switchMap((action: forumAction.SelectTopicAction) => {
            return this.productService.getTopicDetails(action.id)
                .map((data: Topic) => {
                    return new forumAction.SelectTopicCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'getTopicDetails_FAILED' })
                })
        }
    );

    constructor(
        private productService: ProductService,
        private actions$: Actions
    ) { }
}
