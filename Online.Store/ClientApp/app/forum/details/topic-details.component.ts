import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ForumState } from '../store/forum.state';
import { Router, ActivatedRoute, ParamMap, NavigationEnd } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Topic } from "../../models/topic";
import * as forumActions from '../store/forum.actions';

@Component({
    selector: 'topic-details',
    templateUrl: './topic-details.component.html'
})

export class TopicDetailsComponent implements OnInit {

    topic$: Observable<Topic>;

    constructor(private store: Store<any>, private route: ActivatedRoute, private router: Router) {
        this.topic$ = this.store.select<Topic>(state => state.community.forumState.selectedTopic);
    }

    ngOnInit() {
        this.route.paramMap
            .subscribe((params: ParamMap) =>
                this.store.dispatch(new forumActions.SelectTopicAction(params.get('id') || '')));
    }
}