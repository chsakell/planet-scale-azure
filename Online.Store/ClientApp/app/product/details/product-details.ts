import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

@Component({
    selector: 'product-details',
    templateUrl: './product-details.html'
})

export class ProductDetailsComponent implements OnInit {
    constructor(private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap
            .subscribe((params: ParamMap) =>
                console.log(params.get('id')));
    }
}