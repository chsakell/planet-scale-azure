import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ProductState } from '../store/product.state';
import { Observable } from 'rxjs/Observable';
import * as ProductActions from '../store/product.action';
import * as userActions from '../../user/store/user.actions';
import { Product } from "../../models/product";
import { NotifyService } from '../../core/services/notifications.service';

@Component({
    selector: 'product-list',
    templateUrl: './product-list.component.html',
    
})

export class ProductListComponent implements OnInit {
    
    products$: Observable<Product[]>;

    constructor(private store: Store<any>, public notifyService: NotifyService) { 
        this.products$ = this.store.select<Product[]>(state => state.catalog.productState.products);
    }

    ngOnInit() {
        this.notifyService.setLoading(true);
        this.store.dispatch(new ProductActions.SelectAllAction());
     }

     addProductToCart(productId: string) {
        this.store.dispatch(new userActions.AddProductToCartAction(productId));
    }
}