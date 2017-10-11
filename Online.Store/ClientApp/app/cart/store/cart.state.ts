import { Cart } from "../../models/cart";
import { User } from "../../models/user";

export interface CartState {
    cart?: Cart
    user?: User
};