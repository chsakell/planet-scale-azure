import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ProductState } from '../store/product.state';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'product-list',
    templateUrl: './product-list.component.html'
})

export class ProductListComponent implements OnInit {
    
    productState$: Observable<ProductState>;

    constructor(private store: Store<any>) { 
        this.productState$ = this.store.select<ProductState>(state => state.inventory.products);
    }

    ngOnInit() { }
}