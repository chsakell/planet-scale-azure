import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';

import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { of } from 'rxjs/observable/of';
import { Observable } from 'rxjs/Rx';

import * as userActions from './user.actions';
import { Product } from './../../models/product';
import { ProductService } from '../../core/services/product.service';
import { Cart } from "../../models/cart";
import { ResultVM } from '../../models/result-vm';
import { AccountService } from '../../core/services/account.service';

@Injectable()
export class UserEffects {

    @Effect() registerUser: Observable<Action> = this.actions$.ofType(userActions.REGISTER_USER)
    .switchMap((action: userActions.RegisterUserAction) => {
        return this.accountService.registerUser(action.user)
            .map((data: ResultVM) => {
                return new userActions.RegisterUserCompleteAction(data);
            })
            .catch((error: any) => {
                return of({ type: 'registerUser_FAILED' })
            })
    }
    );

    @Effect() loginUser: Observable<Action> = this.actions$.ofType(userActions.LOGIN_USER)
    .switchMap((action: userActions.LoginUserAction) => {
        return this.accountService.loginUser(action.user)
            .map((data: ResultVM) => {
                return new userActions.LoginUserCompleteAction(data);
            })
            .catch((error: any) => {
                return of({ type: 'loginUser_FAILED' })
            })
    }
    );

    @Effect() getCart$: Observable<Action> = this.actions$.ofType(userActions.GET_CART)
        .switchMap(() =>
            this.productService.getCart()
                .map((data: Cart) => {
                    return new userActions.GetCartCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'getCart_FAILED' })
                })
        );

    @Effect() addProductToCart: Observable<Action> = this.actions$.ofType(userActions.ADD_PRODUCT_TO_CART)
        .switchMap((action: userActions.AddProductToCartAction) => {
            return this.productService.addProductToCart(action.id)
                .map((data: Cart) => {
                    return new userActions.AddProductToCartCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'addProductToCart_FAILED' })
                })
        }
        );

        @Effect() completeOrder: Observable<Action> = this.actions$.ofType(userActions.COMPLETE_ORDER)
        .switchMap((action: userActions.CompleteOrderAction) => {
            return this.productService.completeOrder(action.id)
                .map((data: string) => {
                    return new userActions.CompleteOrderCompleteAction(data);
                })
                .catch((error: any) => {
                    return of({ type: 'completeOrder_FAILED' })
                })
        }
        );

    constructor(
        private productService: ProductService,
        private accountService: AccountService,
        private actions$: Actions
    ) { }
}
