import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'product-view',
    template: '<router-outlet></router-outlet>'
})

export class ProductComponent implements OnInit {


    constructor() { }

    ngOnInit() { }
}