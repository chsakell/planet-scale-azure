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

@Injectable()
export class ProductService {

    private productsURI: string;
    private cartsURI: string;
    private headers: Headers;
    private requestOptions: RequestOptions;

    constructor(private http: Http, private configuration: Configuration) {

        this.productsURI = configuration.Server + 'api/products/';
        this.cartsURI = configuration.Server + 'api/carts/';

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

}
