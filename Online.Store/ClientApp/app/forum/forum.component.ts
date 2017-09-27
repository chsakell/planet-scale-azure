import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'forum-view',
    template: '<router-outlet></router-outlet>'
})

export class ForumComponent implements OnInit {


    constructor() { }

    ngOnInit() { }
}