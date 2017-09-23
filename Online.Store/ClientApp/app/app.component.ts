import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { CartState } from './cart/store/cart.state';
import * as CartActions from './cart/store/cart.action';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

    constructor(private store: Store<any>) { }

    ngOnInit(): void {
        this.store.dispatch(new CartActions.GetCartAction());
    }

}
