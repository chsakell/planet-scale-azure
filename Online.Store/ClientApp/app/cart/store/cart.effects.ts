import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';

import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { of } from 'rxjs/observable/of';
import { Observable } from 'rxjs/Rx';

import * as cartAction from './cart.action';
import { Product } from './../../models/product';
import { ProductService } from '../../core/services/product.service';
import { Cart } from "../../models/cart";

@Injectable()
export class CartEffects {

    @Effect() getCart$: Observable<Action> = this.actions$.ofType(cartAction.GET_CART)
        .switchMap(() =>
            this.productService.getCart()
                .map((data: Cart) => {
                    return new cartAction.GetCartCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'getCart_FAILED' })
                })
        );

    @Effect() addProductToCart: Observable<Action> = this.actions$.ofType(cartAction.ADD_PRODUCT_TO_CART)
        .switchMap((action: cartAction.AddProductToCartAction) => {
            return this.productService.addProductToCart(action.id)
                .map((data: Cart) => {
                    return new cartAction.AddProductToCartCompleteAction(data);
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
