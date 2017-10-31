import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';

import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { of } from 'rxjs/observable/of';
import { Observable } from 'rxjs/Rx';

import * as userActions from './user.actions';
import * as notifyActions from '../../notifications/store/notifications.actions';
import { Product } from './../../models/product';
import { ProductService } from '../../core/services/product.service';
import { Cart } from "../../models/cart";
import { ResultVM } from '../../models/result-vm';
import { AccountService } from '../../core/services/account.service';
import { UserCart } from '../../models/user-cart';
import { MessageType } from '../../models/message';
import { SET_MESSAGE } from '../../notifications/store/notifications.actions';

@Injectable()
export class UserEffects {

    @Effect() registerUser: Observable<Action> = this.actions$.ofType(userActions.REGISTER_USER)
    .switchMap((action: userActions.RegisterUserAction) => {
        return this.accountService.registerUser(action.user)
            .mergeMap((data: ResultVM) => {
                return [
                    new notifyActions.SetMessageAction( { type: MessageType.SUCCESS, message: 'Registration completed successfully' }),
                    new userActions.RegisterUserCompleteAction(data)
                ];
            })
            .catch((error: any) => {
                return of(new notifyActions.SetMessageAction( { type: MessageType.Error, message: 'Failed to register user' }))
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
                return of(new notifyActions.SetMessageAction( { type: MessageType.Error, message: 'Failed to signin user' }))
            })
    }
    );

    @Effect() getCart$: Observable<Action> = this.actions$.ofType(userActions.GET_CART)
        .switchMap(() =>
            this.productService.getCart()
                .map((data: UserCart) => {
                    return new userActions.GetCartCompleteAction(data);
                })
                .catch((error: any) => {
                    return of(new notifyActions.SetMessageAction( { type: MessageType.Error, message: 'Failed to load cart' }))
                })
        );

    @Effect() addProductToCart: Observable<Action> = this.actions$.ofType(userActions.ADD_PRODUCT_TO_CART)
        .switchMap((action: userActions.AddProductToCartAction) => {
            return this.productService.addProductToCart(action.id)
                .mergeMap((data: Cart) => {
                    return [
                        new notifyActions.SetMessageAction( { type: MessageType.SUCCESS, message: 'Item added to cart' }),
                        new userActions.AddProductToCartCompleteAction(data)
                    ];
                })
                .catch((error: any) => {
                    return of(new notifyActions.SetMessageAction( { type: MessageType.Error, message: 'Failed to add item to cart' }))
                })
        }
        );
    
    @Effect() removeProductFromCart: Observable<Action> = this.actions$.ofType(userActions.REMOVE_PRODUCT_FROM_CART)
        .switchMap((action: userActions.RemoveProductFromCartAction) => {
            return this.productService.removeProductFromCart(action.id)
                .mergeMap((data: Cart) => {
                    return [
                        new notifyActions.SetMessageAction( { type: MessageType.Info, message: 'Item removed from cart' }),
                        new userActions.RemoveProductFromCartCompleteAction(data)
                    ];
                })
                .catch((error: any) => {
                    return of(new notifyActions.SetMessageAction( { type: MessageType.Error, message: 'Failed to remove item from cart' }))
                })
        }
        );

        @Effect() completeOrder: Observable<Action> = this.actions$.ofType(userActions.COMPLETE_ORDER)
        .switchMap((action: userActions.CompleteOrderAction) => {
            return this.productService.completeOrder(action.id)
                .mergeMap((data: ResultVM) => {
                    return [
                        new notifyActions.SetMessageAction( { type: MessageType.Info, message: 'Your order has been submitted!' }),
                        new userActions.CompleteOrderCompleteAction(data)
                    ];
                })
                .catch((error: any) => {
                    return of(new notifyActions.SetMessageAction( { type: MessageType.Error, message: 'Failed to process request' }))
                })
        }
        );

    constructor(
        private productService: ProductService,
        private accountService: AccountService,
        private actions$: Actions
    ) { }
}
