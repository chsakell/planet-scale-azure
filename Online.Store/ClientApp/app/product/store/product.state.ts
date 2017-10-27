import { Product } from './../../models/product';
import { Cart } from "../../models/cart";

// product state
export interface ProductState {
    products: Product[],
    selectedProduct?: Product,
};