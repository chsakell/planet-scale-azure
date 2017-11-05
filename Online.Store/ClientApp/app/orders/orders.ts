import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
//import { Topic } from "../../models/topic";

@Component({
    selector: 'orders-list-presentation',
    templateUrl: './orders.html',
    styleUrls: ['./orders.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class OrdersPresentationComponent {

    //@Input() topics: Topic[];

    constructor() { }

}