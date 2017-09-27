import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Topic } from "../../models/topic";

@Component({
    selector: 'topic-list-presentation',
    templateUrl: './list.html',
    styleUrls: ['./list.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class TopicDetailsPresentationComponent {

    @Input() topics: Topic[];

    constructor() { }

}