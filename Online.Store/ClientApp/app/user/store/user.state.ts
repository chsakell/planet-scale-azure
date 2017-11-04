import { Cart } from "../../models/cart";
import { User } from "../../models/user";

export interface UserState {
    cart?: Cart,
    cartTotal: number,
    user?: User,
    redirectToLogin: boolean 
};