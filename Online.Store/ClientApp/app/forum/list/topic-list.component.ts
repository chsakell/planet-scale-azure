import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ForumState } from '../store/forum.state';
import { Observable } from 'rxjs/Observable';
import * as forumActions from '../store/forum.actions';
import { Topic } from "../../models/topic";
import { NotifyService } from '../../core/services/notifications.service';
import { User } from '../../models/user';
import { Reply } from '../../models/reply';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Subject } from 'rxjs/Subject';

@Component({
    selector: 'topic-list',
    templateUrl: './topic-list.component.html'
})

export class TopicListComponent implements OnInit {

    topics$: Observable<Map<number, Topic[]>>;
    user$: Observable<User>;
    continuationToken$: Observable<string>;
    selectedPage$: Observable<number>;
    currentPageTopics$: Subject<Topic[]> = new BehaviorSubject(new Array<Topic>());
    totalPages$: Observable<number>;

    constructor(private store: Store<any>, public notifyService: NotifyService) {
        this.topics$ = this.store.select<Map<number, Topic[]>>(state => state.community.forumState.topics);
        this.continuationToken$ = this.store.select<string>(state => state.community.forumState.continuationToken);
        this.user$ = this.store.select<User>(state => state.user.userState.user);
        this.selectedPage$ = this.store.select<number>(state => state.community.forumState.selectedPage);
        this.totalPages$ = this.topics$.map(map => map.size);
    }

    ngOnInit() {
        this.notifyService.setLoading(true);
        this.store.dispatch(new forumActions.SelectAllAction());
        const self = this;
        this.selectedPage$
            .filter(page => page > 0)
            .withLatestFrom(this.topics$)
            .subscribe(([page, topics]) => {
                self.currentPageTopics$.next(topics.get(page));
            });
    }

    submitTopic(reply: Reply) {
        this.notifyService.setLoading(true);
        this.store.dispatch(new forumActions.AddTopicAction(reply));
    }

    getTopics(continuationToken: string) {
        this.notifyService.setLoading(true);
        this.store.dispatch(new forumActions.SelectAllAction(continuationToken));
    }

    getNextPage(data: any) {
        const page = data.page;
        const self = this;

        this.topics$
            .take(1)
            .subscribe((topics) => {
                if(topics.get(page)) {
                    self.store.dispatch(new forumActions.SetSelectedPageAction(page));
                } else {
                    self.getTopics(data.token);
                }
            })
    }

    getPreviousPage(page: number) {
        this.store.dispatch(new forumActions.SetSelectedPageAction(page));
    }

    ngOnDestroy() {
        this.store.dispatch(new forumActions.SetSelectedPageAction(0));
    }
}