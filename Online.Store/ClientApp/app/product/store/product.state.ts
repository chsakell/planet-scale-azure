import { Product } from './../../models/product';
import { Cart } from "../../models/cart";

export interface ProductState {
    products: Product[],
    selectedProduct?: Product,
};