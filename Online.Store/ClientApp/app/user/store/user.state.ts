import { Cart } from "../../models/cart";
import { User } from "../../models/user";

export interface UserState {
    cart?: Cart,
    user?: User,
    selectedPanel: string
};