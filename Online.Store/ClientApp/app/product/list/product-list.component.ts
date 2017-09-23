import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ProductState } from '../store/product.state';
import { Observable } from 'rxjs/Observable';
import * as ProductActions from '../store/product.action';
import { Product } from "../../models/product";

@Component({
    selector: 'product-list',
    templateUrl: './product-list.component.html',
    
})

export class ProductListComponent implements OnInit {
    
    products$: Observable<Product[]>;

    constructor(private store: Store<any>) { 
        this.products$ = this.store.select<Product[]>(state => state.catalog.productState.products);
    }

    ngOnInit() {
        this.store.dispatch(new ProductActions.SelectAllAction());
     }
}