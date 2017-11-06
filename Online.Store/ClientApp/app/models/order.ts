export class Order  {
    public id: number;
    public userId: string;
    public dateCreated: Date;
    public orderDetails: OrderDetail[];
}

export class OrderDetail {
    public id: number;
    public productId: string;
    public productTitle: string;
    public productPrice: number;
    public productImage: string;
    public quantity: number;
}