import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Order } from '../models/order';

@Component({
    selector: 'orders-list-presentation',
    templateUrl: './orders.html',
    styleUrls: ['./orders.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class OrdersPresentationComponent {

    @Input() orders: Order[];

    constructor() { }

}