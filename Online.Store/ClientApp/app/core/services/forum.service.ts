import 'rxjs/add/operator/map';

import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Configuration } from './../../app.constants';
import { Product } from './../../models/product';

// Import RxJs required methods
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Cart } from "../../models/cart";
import { Topic } from '../../models/topic';
import { UserCart } from '../../models/user-cart';
import { ResultVM } from '../../models/result-vm';
import { Order } from '../../models/order';
import { Reply } from '../../models/reply';

@Injectable()
export class ForumService {

    private forumURI: string;
    private headers: Headers;
    private requestOptions: RequestOptions;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {

        this.forumURI = baseUrl + 'api/forum/';

        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');

        this.requestOptions = new RequestOptions({ headers: this.headers })
    }

    addReply(reply: any): Observable<ResultVM> {
        let input = new FormData();
        input.append("file", reply.mediaFile);
        input.append("title", reply.title);
        input.append("content", reply.content);
        input.append("mediaDescription", reply.mediaDescription);
        input.append("userId", reply.userId);
        input.append("username", reply.userName);
        return this.http.post(this.forumURI + `topics/${reply.replyToPostId}/addreply`, input)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }

    createTopic(reply: any): Observable<ResultVM> {
        let input = new FormData();
        input.append("file", reply.mediaFile);
        input.append("title", reply.title);
        input.append("content", reply.content);
        input.append("mediaDescription", reply.mediaDescription);
        input.append("userId", reply.userId);
        input.append("username", reply.userName);
        return this.http.post(this.forumURI + `topics/create`, input)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }
}
