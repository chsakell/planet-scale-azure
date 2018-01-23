import 'rxjs/add/operator/map';

import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Configuration } from './../../app.constants';

// Import RxJs required methods
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { RegisterVM } from '../../models/register-vm';
import { LoginVM } from '../../models/login-vm';
import { ResultVM } from '../../models/result-vm';

@Injectable()
export class AccountService {

    private accountURI: string;
    private identityURI: string;
    private headers: Headers;
    private requestOptions: RequestOptions;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {

        this.accountURI = baseUrl + 'account/';
        this.identityURI = baseUrl + 'api/identity/';

        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');

        this.requestOptions = new RequestOptions({ headers: this.headers })
    }

    registerUser(user: RegisterVM): Observable<ResultVM> {
        debugger;
        let url = user.useIdentity === true ? this.identityURI : this.accountURI;
        return this.http.post(url + 'register', JSON.stringify(user), this.requestOptions)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }

    loginUser(user: LoginVM): Observable<ResultVM> {
        return this.http.post(this.identityURI + 'login', JSON.stringify(user), this.requestOptions)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }

}
