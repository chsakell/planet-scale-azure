import { Cart } from "../../models/cart";
import { User } from "../../models/user";
import { Order } from "../../models/order";

export interface UserState {
    cart?: Cart,
    cartTotal: number,
    user?: User,
    redirectToLogin: boolean,
    orders: Order[],
    useIdentity: boolean 
};