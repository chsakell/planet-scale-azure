import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ForumState } from '../store/forum.state';
import { Router, ActivatedRoute, ParamMap, NavigationEnd } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Topic } from "../../models/topic";
import * as forumActions from '../store/forum.actions';
import { Reply } from '../../models/reply';
import { User } from '../../models/user';
import { NotifyService } from '../../core/services/notifications.service';

@Component({
    selector: 'topic-details',
    templateUrl: './topic-details.component.html'
})

export class TopicDetailsComponent implements OnInit {

    topic$: Observable<Topic>;
    user$: Observable<User>;

    constructor(private store: Store<any>, private notifyService: NotifyService,
        private route: ActivatedRoute, private router: Router) {
        this.topic$ = this.store.select<Topic>(state => state.community.forumState.selectedTopic);
        this.user$ = this.store.select<User>(state => state.user.userState.user);
    }

    ngOnInit() {
        this.route.paramMap
            .subscribe((params: ParamMap) => {
                this.notifyService.setLoading(true);
                this.store.dispatch(new forumActions.SelectTopicAction(params.get('id') || '')
                )
            });
    }

    submitReply(reply: Reply) {
        this.notifyService.setLoading(true);
        this.store.dispatch(new forumActions.AddReplyAction(reply));
    }
}