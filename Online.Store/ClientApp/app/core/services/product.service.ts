import 'rxjs/add/operator/map';

import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Configuration } from './../../app.constants';
import { Product } from './../../models/product';

// Import RxJs required methods
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

@Injectable()
export class ProductService {

    private actionUrl: string;
    private headers: Headers;
    private requestOptions: RequestOptions;

    constructor(private http: Http, private configuration: Configuration) {

        this.actionUrl = configuration.Server + 'api/things/';

        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');

        this.requestOptions = new RequestOptions({ headers: this.headers })

    }

    getAll(): Observable<Product[]> {
        return this.http.get(this.actionUrl, this.requestOptions)
            // ...and calling .json() on the response to return data
            .map((res: Response) => res.json())
            //...errors if any
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));;
    }

    getSingle(id: number): Observable<Product> {
        return this.http.get(this.actionUrl + id, this.requestOptions)
            // ...and calling .json() on the response to return data
            .map((res: Response) => res.json())
            //...errors if any
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }

}
