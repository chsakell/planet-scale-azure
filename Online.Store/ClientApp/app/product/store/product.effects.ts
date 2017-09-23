import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';

import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { of } from 'rxjs/observable/of';
import { Observable } from 'rxjs/Rx';

import * as productsAction from './product.action';
import { Product } from './../../models/product';
import { ProductService } from '../../core/services/product.service';
import { Cart } from "../../models/cart";

@Injectable()
export class ProductEffects {

    @Effect() getAll$: Observable<Action> = this.actions$.ofType(productsAction.SELECTALL)
        .switchMap(() =>
            this.productService.getAll()
                .map((data: Product[]) => {
                    return new productsAction.SelectAllCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'getAll_FAILED' })
                })
        );

    @Effect() getProduct$: Observable<Action> = this.actions$.ofType(productsAction.SELECT_PRODUCT)
        .switchMap((action: productsAction.SelectProductAction) => {
            return this.productService.getSingle(action.id)
                .map((data: Product) => {
                    return new productsAction.SelectProductCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'getProduct_FAILED' })
                })
        }
        );

    @Effect() addProductToCart: Observable<Action> = this.actions$.ofType(productsAction.ADD_PRODUCT_TO_CART)
        .switchMap((action: productsAction.AddProductToCartAction) => {
            return this.productService.addProductToCart(action.id)
                .map((data: Cart) => {
                    return new productsAction.AddProductToCartCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'addProductToCart_FAILED' })
                })
        }
        );

    constructor(
        private productService: ProductService,
        private actions$: Actions
    ) { }
}
