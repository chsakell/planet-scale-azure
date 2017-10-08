import 'rxjs/add/operator/map';

import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Configuration } from './../../app.constants';
import { Product } from './../../models/product';

// Import RxJs required methods
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Cart } from "../../models/cart";
import { Topic } from '../../models/topic';

@Injectable()
export class ProductService {

    private productsURI: string;
    private forumURI: string;
    private cartsURI: string;
    private ordersURI: string;
    private headers: Headers;
    private requestOptions: RequestOptions;

    constructor(private http: Http, private configuration: Configuration) {

        this.productsURI = configuration.Server + 'api/products/';
        this.forumURI = configuration.Server + 'api/forum/topics/';
        this.cartsURI = configuration.Server + 'api/carts/';
        this.ordersURI = configuration.Server + 'api/orders/';

        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');

        this.requestOptions = new RequestOptions({ headers: this.headers })
    }

    getAll(): Observable<Product[]> {
        return this.http.get(this.productsURI, this.requestOptions)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));;
    }

    getSingle(id: string): Observable<Product> {
        return this.http.get(this.productsURI + id, this.requestOptions)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }

    getCart(): Observable<Cart> {
        return this.http.get(this.cartsURI, this.requestOptions)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));;
    }

    addProductToCart(id: string): Observable<Cart> {
        return this.http.post(this.cartsURI, '"' + id + '"', this.requestOptions)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }

    completeOrder(id: string): Observable<string> {
        return this.http.post(this.ordersURI, '"' + id + '"', this.requestOptions)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }

    getTopics(): Observable<Topic[]> {
        return this.http.get(this.forumURI, this.requestOptions)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));;
    }

    getTopicDetails(id: string): Observable<Topic> {
        return this.http.get(this.forumURI + id, this.requestOptions)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }

}
