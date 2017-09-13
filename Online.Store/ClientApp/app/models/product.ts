
import { ProductComponent } from "./product-component";

export class Product {
    public productId: string;
    public productPrice: number;
    public productHeading: string;
    public description: string;
    public productURL: string;
    public productComponentsList: ProductComponent[];
}