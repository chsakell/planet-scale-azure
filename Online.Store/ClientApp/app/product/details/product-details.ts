import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Router, ActivatedRoute, ParamMap, NavigationEnd } from '@angular/router';
import { Observable } from "rxjs/Observable";
import * as ProductActions from '../store/product.action';
import * as CartActions from '../../cart/store/cart.action';
import { Product } from "../../models/product";

@Component({
    selector: 'product-details',
    templateUrl: './product-details.html'
})

export class ProductDetailsComponent implements OnInit {

    product$: Observable<Product>;

    constructor(private store: Store<any>, private route: ActivatedRoute, private router: Router) {
        this.product$ = this.store.select<Product>(state => state.catalog.productState.selectedProduct);
    }

    ngOnInit() {
        this.route.paramMap
            .subscribe((params: ParamMap) =>
                this.store.dispatch(new ProductActions.SelectProductAction(params.get('id') || '')));
    }

    addProductToCart(productId: string) {
        this.store.dispatch(new CartActions.AddProductToCartAction(productId));
    }
}