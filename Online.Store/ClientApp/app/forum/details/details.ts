import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Topic } from "../../models/topic";

@Component({
    selector: 'topic-details-presentation',
    templateUrl: './details.html',
    styleUrls: ['./details.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class TopicDetailsPresentationComponent {

    @Input() topics: Topic[];

    constructor() { }

}